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

        public Distance PressureAltitude
        {
            get
            {

                return Distance.FromFeet(
                    CalculationUtilities.PressureAltitudeAtFieldElevation(QNH, Airport.FieldElevation)
                    );

            }
        }

        public Distance PressureAltitudeAlwaysPositiveOrZero
        {
            get
            {
                return PressureAltitude.TotalFeet >= 0 ? PressureAltitude : Distance.FromFeet(0);
            }
        }

        public decimal AircraftWeight
        {
            get;
            set;
        }

        public Distance DensityAltitude
        {
            get
            {
                return
                Distance.FromFeet(
                    CalculationUtilities.DensityAltitudeAtAirport(TemperatureCelcius, ISATemperature, PressureAltitude.TotalFeet));

            }
        }

        public decimal ISATemperature
        {
            get
            {
                return CalculationUtilities.ISATemperatureForPressureAltitude(PressureAltitude.TotalFeet);
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
            if (PressureAltitude.TotalFeet < 0)
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

                GetRunwaySlope(db, runway);

                // calculate the performance for this runway and aircraft
                Results.Add(CalculatePerformanceForRunway(runway));
            }

        }

        private void GetRunwaySlope(AirportDbContext db, Runway runway)
        {
            if (runway.RunwayLength != null)
            {
                if (db.Runways.Count(r => r.Airport_Ident == Airport.Ident.ToUpper() && r.Runway_Name == Runway.GetOppositeRunway(runway.Runway_Name)) == 0)
                {
                    // if there is no opposite runway, we can't calculate the slope
                    runway.Slope = null;
                }
                else
                {
                    // determine the opposite runway
                    Runway oppositeRunway = db.Runways.Where(r => r.Airport_Ident == Airport.Ident.ToUpper() && r.Runway_Name == Runway.GetOppositeRunway(runway.Runway_Name)).First<Runway>();

                    if (runway.ElevationFt_Converted.HasValue && oppositeRunway.ElevationFt_Converted.HasValue && runway.RunwayLengthConverted.HasValue)
                    {
                        runway.Slope = (decimal)Runway.CalculateSlope(
                            runway.ElevationFt_Converted.Value,
                            oppositeRunway.ElevationFt_Converted.Value,
                            (double)runway.RunwayLengthConverted.Value);
                    }
                }
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

            PerformanceCalculationLogItem firstItem = new PerformanceCalculationLogItem(
                $"Looking for aircraft performance data in these JSON files:");
            firstItem.Add($"File: {Aircraft.JsonFileName_LowerWeight()}");
            firstItem.Add($"File: {Aircraft.JsonFileName_HigherWeight()}");
            _logger.Add(firstItem);


            PopulateInterpolatedTakeoffData(bookPerformanceDataList);
            PopulateInterpolatedLandingData(bookPerformanceDataList);

        }

        private void PopulateInterpolatedTakeoffData(BookPerformanceDataList bookPerformanceDataList)
        {

            PerformanceCalculationLogItem takeoffLogger = new PerformanceCalculationLogItem(
                "Getting takeoff performance from the POH table and interpolating:"
                );
            _logger.Add(takeoffLogger);

            // get the book takeoff performance data
            PeformanceDataInterpolator takeoffInterpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                PressureAltitudeAlwaysPositiveOrZero,
                TemperatureCelciusAlwaysPositiveOrZero,
                AircraftWeight,
                bookPerformanceDataList,
                takeoffLogger);

            // interpolate the book takeoff performance data
            IntepolatedTakeoffPerformanceData =
                takeoffInterpolator.GetInterpolatedBookDistances(
                    Aircraft);
        }

        private void PopulateInterpolatedLandingData(BookPerformanceDataList bookPerformanceDataList)
        {
            // get the book landing performance data
            PerformanceCalculationLogItem landingLogger = new PerformanceCalculationLogItem(
                "Getting landing performance from the POH table and interpolating:"
                );
            _logger.Add(landingLogger);

            // get the book landing performance data
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
            PerformanceCalculationLogItem runwayLogger =
                new PerformanceCalculationLogItem($"Calculating performance for runway {runway.Runway_Name}");

            PerformanceCalculationResultForRunway result =
                new PerformanceCalculationResultForRunway(runway, Wind, runwayLogger, PressureAltitudeAlwaysPositiveOrZero);

            CalculateTakeoffPerformanceForRunway(runway, runwayLogger, result);
            CalculateLandingPerformanceForRunway(runway, runwayLogger, result);

            return result;
        }

        private void CalculateLandingPerformanceForRunway(Runway runway, PerformanceCalculationLogItem runwayLogger, PerformanceCalculationResultForRunway result)
        {
            PerformanceCalculationLogItem landingAdjustmentsLogger = new PerformanceCalculationLogItem(
                $"Making landing adjustments for runway {runway.Runway_Name}");

            runwayLogger.SubItems.Add(landingAdjustmentsLogger);

            PerformanceCalculationLogItem landingGroundRollLogger = new PerformanceCalculationLogItem(
                $"Making landing ground roll adjustments for runway {runway.Runway_Name}");

            landingAdjustmentsLogger.SubItems.Add(landingGroundRollLogger);

            result.Landing_GroundRoll = Aircraft.MakeLandingAdjustments
                (
                    result,
                    IntepolatedLandingPerformanceData.DistanceGroundRoll.Value,
                    landingGroundRollLogger
                );

            PerformanceCalculationLogItem landing50FtClearanceLogger = new PerformanceCalculationLogItem(
                $"Making landing 50 ft clearance adjustments for runway {runway.Runway_Name}");

            landingAdjustmentsLogger.SubItems.Add(landing50FtClearanceLogger);

            result.Landing_50FtClearance = Aircraft.MakeLandingAdjustments
                (
                    result,
                    IntepolatedLandingPerformanceData.DistanceToClear50Ft.Value,
                    landing50FtClearanceLogger
                );
        }

        private void CalculateTakeoffPerformanceForRunway(Runway runway, PerformanceCalculationLogItem runwayLogger, PerformanceCalculationResultForRunway result)
        {
            PerformanceCalculationLogItem takeoffAdjustmentsLogger = new PerformanceCalculationLogItem(
                            $"Making takeoff adjustments for runway {runway.Runway_Name}");

            runwayLogger.SubItems.Add(takeoffAdjustmentsLogger);

            PerformanceCalculationLogItem takeoffGroundRollLogger = new PerformanceCalculationLogItem(
                $"Making takeoff ground roll adjustments for runway {runway.Runway_Name}");

            takeoffAdjustmentsLogger.SubItems.Add(takeoffGroundRollLogger);

            result.Takeoff_GroundRoll =
                Aircraft.MakeTakeoffAdjustments
                (
                    result,
                    IntepolatedTakeoffPerformanceData.DistanceGroundRoll.Value,
                    takeoffGroundRollLogger
                );

            PerformanceCalculationLogItem takeoff50FtClearanceLogger = new PerformanceCalculationLogItem(
                $"Making takeoff 50 ft clearance adjustments for runway {runway.Runway_Name}");

            takeoffAdjustmentsLogger.SubItems.Add(takeoff50FtClearanceLogger);

            result.Takeoff_50FtClearance = Aircraft.MakeTakeoffAdjustments
                (
                    result,
                    IntepolatedTakeoffPerformanceData.DistanceToClear50Ft.Value,
                    takeoff50FtClearanceLogger
                );
        }
    }
}