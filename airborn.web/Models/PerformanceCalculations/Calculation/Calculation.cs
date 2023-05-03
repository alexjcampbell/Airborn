using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Airborn.web.Models.PerformanceData;

namespace Airborn.web.Models
{

    /// <summary>
    /// This class is responsible for calculating the performance of a given aircraft at a 
    /// given airport in a given set of conditions.
    /// </summary>
    public class Calculation
    {

        private Calculation()
        {
        }


        public Calculation(
            Aircraft aircraft,
            Airport airport,
            Wind wind,
            double aircraftWeight,
            string path)
        {
            Aircraft = aircraft;
            Airport = airport;
            _wind = wind;
            AircraftWeight = aircraftWeight;
            JsonPath = path;

        }

        public Calculation(
            Aircraft aircraft,
            Airport airport,
            Wind wind,
            double aircraftWeight,
            JsonFile jsonFile)
        {
            Aircraft = aircraft;
            Airport = airport;
            _wind = wind;
            AircraftWeight = aircraftWeight;
            JsonFile = jsonFile;
        }

        /// <summary>
        /// The path to the JSON file that contains the performance data for the aircraft,
        /// used by the UI.
        /// </summary>
        public string JsonPath
        {
            get;
            set;
        }

        /// <summary>
        /// The actual JSON file that contains the performance data for the aircraft,
        /// used by unit tests
        /// </summary>
        public JsonFile JsonFile
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

        private List<CalculationResultForRunway> _results = new List<CalculationResultForRunway>();

        public List<CalculationResultForRunway> Results
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

        public double TemperatureCelcius
        {
            get;
            set;
        }

        public double TemperatureCelciusAlwaysPositiveOrZero
        {
            get
            {
                return TemperatureCelcius >= 0 ? TemperatureCelcius : 0;
            }
        }


        public double AltimeterSettingInMb
        {
            get;
            set;
        }

        public Distance PressureAltitude
        {
            get
            {

                return Distance.FromFeet(
                    CalculationUtilities.PressureAltitudeAtFieldElevation(AltimeterSettingInMb, Airport.FieldElevation.Value)
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

        public double AircraftWeight
        {
            get;
            set;
        }

        public Distance DensityAltitude
        {
            get
            {
                return Distance.FromFeet(
                    CalculationUtilities.DensityAltitudeAtAirport(
                        TemperatureCelcius,
                        ISATemperature,
                        PressureAltitude.TotalFeet
                        )
                    );

            }
        }

        public double ISATemperature
        {
            get
            {
                return CalculationUtilities.ISATemperatureForPressureAltitude(
                    PressureAltitude.TotalFeet
                    );
            }
        }

        public Aircraft Aircraft
        {
            get;
            private set;
        }

        // notes to return to the UI about the calculations (e.g. that pressure altitude is < 0 so we're using 0)
        private List<string> _notes = new List<string>();

        public List<string> Notes
        {
            get
            {
                return _notes;
            }
        }

        public InterpolatedPerformanceData IntepolatedTakeoffPerformanceData
        {
            get;
            private set;
        }

        public InterpolatedPerformanceData IntepolatedLandingPerformanceData
        {
            get;
            private set;
        }

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

            List<CalculationResultForRunway> results = new List<CalculationResultForRunway>();

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
                    Runway oppositeRunway = db.Runways.Where(
                            r => r.Airport_Ident == Airport.Ident.ToUpper()
                            &&
                            r.Runway_Name == Runway.GetOppositeRunway(runway.Runway_Name)
                        ).First<Runway>();

                    if (runway.ElevationFt.HasValue && oppositeRunway.ElevationFt.HasValue && runway.RunwayLength.HasValue)
                    {
                        runway.Slope = (double)Runway.CalculateSlope(
                            runway.ElevationFt.Value,
                            oppositeRunway.ElevationFt.Value,
                            (double)runway.RunwayLength.Value);
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


            if (JsonFile != null) // only used for unit testing
            {
                // populate the list of book performance data from the JSON file provided
                bookPerformanceDataList.PopulateFromJson(Aircraft, JsonFile, JsonFile);
            }
            else if (JsonPath != null)
            {
                // populate the list of book performance data by reading the JSON file
                bookPerformanceDataList.PopulateFromJsonStringPath(Aircraft, JsonPath);
            }
            else
            {
                throw new ArgumentNullException("JsonPath and JsonFile cannot both be null");
            }


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
        private CalculationResultForRunway CalculatePerformanceForRunway(Runway runway)
        {
            PerformanceCalculationLogItem runwayLogger =
                new PerformanceCalculationLogItem($"Calculating performance for runway {runway.Runway_Name}");

            CalculationResultForRunway result =
                new CalculationResultForRunway(runway, Wind, runwayLogger, PressureAltitudeAlwaysPositiveOrZero);

            CalculateTakeoffPerformanceForRunway(runway, runwayLogger, result);
            CalculateLandingPerformanceForRunway(runway, runwayLogger, result);

            return result;
        }


        private void CalculateTakeoffPerformanceForRunway(Runway runway, PerformanceCalculationLogItem runwayLogger, CalculationResultForRunway result)
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

        private void CalculateLandingPerformanceForRunway(Runway runway, PerformanceCalculationLogItem runwayLogger, CalculationResultForRunway result)
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

    }
}