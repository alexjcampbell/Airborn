using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;


namespace Airborn.web.Models
{

    public class PerformanceCalculator
    {

        private PerformanceCalculator()
        {
        }


        public PerformanceCalculator(
            Aircraft aircraft,
            Airport airport,
            Wind wind,
            string path)
        {
            Aircraft = aircraft;
            Airport = airport;
            _wind = wind;
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

        private List<PerformanceCalculationResult> _results = new List<PerformanceCalculationResult>();

        public List<PerformanceCalculationResult> Results
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
                return CalculationUtilities.CalculatePressureAltitudeAtFieldElevation(QNH, Airport.FieldElevation);
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
                return CalculationUtilities.CalculateDensityAltitudeAtAirport(TemperatureCelcius, ISATemperature, PressureAltitude);
            }
        }

        public decimal ISATemperature
        {
            get
            {
                return CalculationUtilities.CalculateISATemperatureForPressureAltitude(PressureAltitude);
            }
        }

        public Aircraft Aircraft
        {
            get;
            private set;
        }

        // notes to return to the UI about the calculations
        public List<string> Notes = new List<string>();

        public InterpolatedPerformanceData IntepolatedTakeoffPerformanceData;
        public InterpolatedPerformanceData IntepolatedLandingPerformanceData;

        public void Calculate(AirportDbContext db)
        {

            if (PressureAltitude < 0)
            {
                Notes.Add("Pressure altitude is negative. The POH only provides performance data for positive pressure altitudes, so we'll calculate based on a pressure altitude of 0 ft (sea level). Actual performance should be better than the numbers stated here.");
            }

            if (TemperatureCelcius < 0)
            {
                Notes.Add("Temperature is negative. The POH data only provides performance date for positive temperatures, so we'll calculate based on a temperature of zero degrees C. Actual performance should be better than the numbers stated here.");
            }

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(AircraftType);

            PopulateInterpolatedPerformanceData(aircraft);

            List<PerformanceCalculationResult> results = new List<PerformanceCalculationResult>();

            foreach (Runway runway in
                db.Runways.Where(runway => runway.Airport_Ident == Airport.Ident.ToUpper()).ToList<Runway>())
            {
                Runway primaryRunway = runway;

                // RunwayIdentifer could be 10R or 28L so we use a regex to get only the first two characters
                SetRunwayHeading(runway, primaryRunway);

                Runways.Add(primaryRunway);
            }

            foreach (Runway runway in Runways)
            {
                PerformanceCalculationResult result = CalculatePerformanceForRunway(aircraft, runway);

                Results.Add(result);
            }

        }

        private void PopulateInterpolatedPerformanceData(Aircraft aircraft)
        {
            BookPerformanceDataList bookPerformanceDataList = new BookPerformanceDataList(aircraft.GetLowerWeight(), aircraft.GetHigherWeight());
            bookPerformanceDataList.PopulateFromJsonStringPath(aircraft, JsonPath);

            // get the lower weight data
            PeformanceDataInterpolator takeoffInterpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                PressureAltitudeAlwaysPositiveOrZero,
                TemperatureCelciusAlwaysPositiveOrZero,
                (decimal)AircraftWeight,
                bookPerformanceDataList);

            IntepolatedTakeoffPerformanceData =
                takeoffInterpolator.GetInterpolatedBookDistance(
                    aircraft);


            PeformanceDataInterpolator landingInterpolator = new PeformanceDataInterpolator(
                Scenario.Landing,
                PressureAltitudeAlwaysPositiveOrZero,
                TemperatureCelciusAlwaysPositiveOrZero,
                (decimal)AircraftWeight,
                bookPerformanceDataList);

            IntepolatedLandingPerformanceData =
                landingInterpolator.GetInterpolatedBookDistance(
                    aircraft);

        }

        private static void SetRunwayHeading(Runway runway, Runway primaryRunway)
        {
            string runwayName = Regex.Replace(runway.Runway_Name, @"\D+", "");

            primaryRunway.RunwayHeading =
                new Direction(
                    int.Parse(runwayName) * 10,
                    0
                    );
        }

        /// <summary>
        /// Calculates the performance for a given runway
        /// </summary>
        private PerformanceCalculationResult CalculatePerformanceForRunway(Aircraft aircraft, Runway runway)
        {
            PerformanceCalculationResult result =
                new PerformanceCalculationResult(runway, Wind);


            result.Takeoff_GroundRoll =
                aircraft.MakeTakeoffAdjustments
                (
                    result,
                    IntepolatedTakeoffPerformanceData.DistanceGroundRoll.Value
                );

            result.Takeoff_50FtClearance = aircraft.MakeTakeoffAdjustments
                (
                    result,
                    IntepolatedTakeoffPerformanceData.DistanceToClear50Ft.Value
                );

            result.Landing_GroundRoll = aircraft.MakeLandingAdjustments
                (
                    result,
                    IntepolatedLandingPerformanceData.DistanceGroundRoll.Value
                );

            result.Landing_50FtClearance = aircraft.MakeLandingAdjustments
                (
                    result,
                    IntepolatedLandingPerformanceData.DistanceToClear50Ft.Value
                );

            return result;
        }

    }
}