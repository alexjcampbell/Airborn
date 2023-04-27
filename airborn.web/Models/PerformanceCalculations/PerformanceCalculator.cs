using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;


namespace Airborn.web.Models
{

    /// <summary>
    /// This class is responsible for calculating the performance of a given aircraft at a 
    /// given airport in a given set of conditions.
    /// </summary>
    public class PerformanceCalculator
    {

        private PerformanceCalculator()
        {
        }


        public PerformanceCalculator(
            Aircraft aircraft,
            Airport airport,
            Wind wind,
            decimal aircraftWeight,
            string path)
        {
            Aircraft = aircraft;
            Airport = airport;
            _wind = wind;
            AircraftWeight = aircraftWeight;
            JsonPath = path;

        }

        public string JsonPath
        {
            get;
            set;
        }

        public Airport Airport
        {
            get;
        }

        public AircraftType AircraftType
        {
            get;
        }

        private List<Runway> _runways = new List<Runway>();

        public List<Runway> Runways
        {
            get
            {
                return _runways;
            }

        }

        private List<PerformanceCalculationResultForRunway> _results = new List<PerformanceCalculationResultForRunway>();

        public List<PerformanceCalculationResultForRunway> Results
        {
            get
            {
                return _results;
            }
        }

        private Wind _wind;

        public Wind Wind
        {
            get
            {
                return _wind;
            }
        }

        public decimal TemperatureCelcius
        {
            get;
            set;
        }

        public decimal TemperatureCelciusAlwaysPositiveOrZero
        {
            get
            {
                return TemperatureCelcius >= 0 ? TemperatureCelcius : 0;
            }
        }


        public decimal QNH
        {
            get;
            set;
        }

        public decimal PressureAltitude
        {
            get
            {
                return CalculationUtilities.PressureAltitudeAtFieldElevation(QNH, Airport.FieldElevation);
            }
        }

        public decimal PressureAltitudeAlwaysPositiveOrZero
        {
            get
            {
                return PressureAltitude >= 0 ? PressureAltitude : 0;
            }
        }

        public decimal AircraftWeight
        {
            get;
            set;
        }

        public decimal DensityAltitude
        {
            get
            {
                return CalculationUtilities.DensityAltitudeAtAirport(TemperatureCelcius, ISATemperature, PressureAltitude);
            }
        }

        public decimal ISATemperature
        {
            get
            {
                return CalculationUtilities.ISATemperatureForPressureAltitude(PressureAltitude);
            }
        }

        public Aircraft Aircraft
        {
            get;
            private set;
        }

        // notes to return to the UI about the calculations (e.g. that pressure altitude is < 0 so we're using 0)
        public List<string> Notes = new List<string>();

        public InterpolatedPerformanceData IntepolatedTakeoffPerformanceData;
        public InterpolatedPerformanceData IntepolatedLandingPerformanceData;

        private PerformanceCalculationLogger _logger = new PerformanceCalculationLogger();

        public PerformanceCalculationLogger Logger
        {
            get
            {
                return _logger;
            }
        }

        public void Calculate(AirportDbContext db)
        {

            // if the pressure altitude is negative, we'll use zero for the calculations
            if (PressureAltitude < 0)
            {
                Notes.Add("Pressure altitude is negative. The POH only provides performance data for positive pressure altitudes, so we'll calculate based on a pressure altitude of 0 ft (sea level). Actual performance should be better than the numbers stated here.");
            }

            // if the temperature is negative, we'll use zero degrees C for the calculations
            if (TemperatureCelcius < 0)
            {
                Notes.Add("Temperature is negative. The POH data only provides performance date for positive temperatures, so we'll calculate based on a temperature of zero degrees C. Actual performance should be better than the numbers stated here.");
            }

            PopulateInterpolatedPerformanceData();

            List<PerformanceCalculationResultForRunway> results = new List<PerformanceCalculationResultForRunway>();

            foreach (Runway runway in
                db.Runways.Where(runway => runway.Airport_Ident == Airport.Ident.ToUpper()).ToList<Runway>())
            {
                Runways.Add(runway);

                // calculate the performance for this runway and aircraft
                Results.Add(CalculatePerformanceForRunway(runway));
            }

        }

        /// <summary>
        /// Populates the interpolated takeoff and landing distances for the given aircraft
        /// </summary>
        private void PopulateInterpolatedPerformanceData()
        {
            BookPerformanceDataList bookPerformanceDataList =
                new BookPerformanceDataList(Aircraft.GetLowerWeight(), Aircraft.GetHigherWeight());

            // populate the list of book performance data from the JSON file
            bookPerformanceDataList.PopulateFromJsonStringPath(Aircraft, JsonPath);

            _logger.Add($"Book performance data list populated from JSON file {Aircraft.JsonFileName_LowerWeight()} and {Aircraft.JsonFileName_HigherWeight()}");

            PerformanceCalculationLogItem takeoffLogger = new PerformanceCalculationLogItem("Getting takeoff performance from the POH table, and interpolating:");
            _logger.Add(takeoffLogger);

            PeformanceDataInterpolator takeoffInterpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                PressureAltitudeAlwaysPositiveOrZero,
                TemperatureCelciusAlwaysPositiveOrZero,
                AircraftWeight,
                bookPerformanceDataList,
                takeoffLogger);

            IntepolatedTakeoffPerformanceData =
                takeoffInterpolator.GetInterpolatedBookDistances(
                    Aircraft);

            PerformanceCalculationLogItem landingLogger = new PerformanceCalculationLogItem("Calculating landing performance from the POH table, and interpolating:");
            _logger.Add(landingLogger);

            PeformanceDataInterpolator landingInterpolator = new PeformanceDataInterpolator(
                Scenario.Landing,
                PressureAltitudeAlwaysPositiveOrZero,
                TemperatureCelciusAlwaysPositiveOrZero,
                AircraftWeight,
                bookPerformanceDataList,
                landingLogger);

            IntepolatedLandingPerformanceData =
                landingInterpolator.GetInterpolatedBookDistances(
                    Aircraft);

        }

        /// <summary>
        /// Takes the book numbers and adjusts them for the given runway and wind conditions
        /// </summary>
        private PerformanceCalculationResultForRunway CalculatePerformanceForRunway(Runway runway)
        {
            PerformanceCalculationResultForRunway result =
                new PerformanceCalculationResultForRunway(runway, Wind);

            result.Takeoff_GroundRoll =
                Aircraft.MakeTakeoffAdjustments
                (
                    result,
                    IntepolatedTakeoffPerformanceData.DistanceGroundRoll.Value
                );

            result.Takeoff_50FtClearance = Aircraft.MakeTakeoffAdjustments
                (
                    result,
                    IntepolatedTakeoffPerformanceData.DistanceToClear50Ft.Value
                );

            result.Landing_GroundRoll = Aircraft.MakeLandingAdjustments
                (
                    result,
                    IntepolatedLandingPerformanceData.DistanceGroundRoll.Value
                );

            result.Landing_50FtClearance = Aircraft.MakeLandingAdjustments
                (
                    result,
                    IntepolatedLandingPerformanceData.DistanceToClear50Ft.Value
                );

            return result;
        }

    }
}