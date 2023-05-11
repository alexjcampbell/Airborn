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
            private set;
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

        public double TemperatureCelciusAdjustedForBounds
        {
            get
            {
                if (TemperatureCelcius < 0)
                {
                    // if the temperature is below zero, just use zero as that's 
                    // the lowest temperature we have performance data for
                    return 0;
                }
                else if (TemperatureCelcius > Aircraft.HighestPossibleTemperature)
                {
                    return Aircraft.HighestPossibleTemperature;
                }
                else
                {
                    return TemperatureCelcius;
                }
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

        public Distance PressureAltitudeAdjustedForBounds
        {
            get
            {
                if (PressureAltitude.TotalFeet < 0)
                {
                    // if the pressure altitude is below zero, just use zero as that's 
                    // the lowest pressure altitude we have performance data for
                    return Distance.FromFeet(0);
                }
                else if (PressureAltitude.TotalFeet > Aircraft.HighestPossiblePressureAltitude)
                {
                    return Distance.FromFeet(Aircraft.HighestPossiblePressureAltitude);
                }
                else
                {
                    return PressureAltitude;
                }
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

        public AirconOptions AirconOption
        {
            get;
            set;
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

        private PerformanceCalculationLog _logger = new PerformanceCalculationLog();

        public PerformanceCalculationLog Logger
        {
            get
            {
                return _logger;
            }
        }

        public void Calculate(AirbornDbContext dbContext)
        {

            // if the pressure altitude is negative, we'll use zero for the calculations
            if (PressureAltitude.TotalFeet < 0)
            {
                Notes.Add("Pressure altitude is negative. The POH provides performance data for positive pressure altitudes, so we'll calculate based on a pressure altitude of 0 ft (sea level). Actual performance should be better than the numbers stated here.");
            }
            else if (PressureAltitude.TotalFeet > Aircraft.HighestPossiblePressureAltitude)
            {
                Notes.Add($"Pressure altitude is {PressureAltitude.TotalFeet} ft, but POH data only provides performance information for pressure altitudes up to {Aircraft.HighestPossiblePressureAltitude} ft, so we'll calculate based on a pressure altitude of {Aircraft.HighestPossiblePressureAltitude} ft. Actual performance will be worse than the numbers stated here.");
            }

            // if the temperature is negative, we'll use zero degrees C for the calculations
            if (TemperatureCelcius < 0)
            {
                Notes.Add("Temperature is negative. The POH only provides performance data for positive temperatures, so we'll calculate based on a temperature of zero degrees C. Actual performance should be better than the numbers stated here.");
            }
            else if (
                TemperatureCelcius > Aircraft.HighestPossibleTemperature
                )
            {
                Notes.Add($"Temperature is {TemperatureCelcius}, but POH data only provides performance information for temperatures up to {Aircraft.HighestPossibleTemperature} degrees C, so we'll calculate based on a temperature of {Aircraft.HighestPossibleTemperature} degrees C. Actual performance will be worse than the numbers stated here.");
            }

            PopulateInterpolatedPerformanceData();

            List<CalculationResultForRunway> results = new List<CalculationResultForRunway>();

            Airport = dbContext.GetAirport(Airport.Ident.ToUpper());

            foreach (Runway runway in Airport.UsableRunways)
            {

                runway.Airport = Airport;

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
                new BookPerformanceDataList(Aircraft.LowestPossibleWeight, Aircraft.HighestPossibleWeight);


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


            PerformanceCalculationLog.LogItem firstItem = _logger.NewItem(
                $"Looking for aircraft performance data in these JSON files:");
            firstItem.AddSubItem($"File: {Aircraft.JsonFileName_LowestWeight}");
            firstItem.AddSubItem($"File: {Aircraft.JsonFileName_HighestWeight}");

            PopulateInterpolatedTakeoffData(bookPerformanceDataList);
            PopulateInterpolatedLandingData(bookPerformanceDataList);

        }

        private void PopulateInterpolatedTakeoffData(BookPerformanceDataList bookPerformanceDataList)
        {

            PerformanceCalculationLog.LogItem takeoffLogger = _logger.NewItem(
                "Getting takeoff performance from the POH table and interpolating:"
                );

            // get the book takeoff performance data
            PeformanceDataInterpolator takeoffInterpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                PressureAltitudeAdjustedForBounds,
                TemperatureCelciusAdjustedForBounds,
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
            PerformanceCalculationLog.LogItem landingLogger = _logger.NewItem(
                "Getting landing performance from the POH table and interpolating:"
                );

            // get the book landing performance data
            PeformanceDataInterpolator landingInterpolator = new PeformanceDataInterpolator(
                Scenario.Landing,
                PressureAltitudeAdjustedForBounds,
                TemperatureCelciusAdjustedForBounds,
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
            PerformanceCalculationLog.LogItem runwayLogger =
                new PerformanceCalculationLog.LogItem($"Calculating performance for runway {runway.Runway_Name}");

            CalculationResultForRunway result =
                new CalculationResultForRunway(runway, Wind, runwayLogger, PressureAltitudeAdjustedForBounds);

            CalculateTakeoffPerformanceForRunway(runway, runwayLogger, result);
            CalculateLandingPerformanceForRunway(runway, runwayLogger, result);

            return result;
        }


        private void CalculateTakeoffPerformanceForRunway(Runway runway, PerformanceCalculationLog.LogItem runwayLogger, CalculationResultForRunway result)
        {
            PerformanceCalculationLog.LogItem takeoffAdjustmentsLogger = runwayLogger.NewItem(
                $"Making takeoff adjustments for runway {runway.Runway_Name}");

            PerformanceCalculationLog.LogItem takeoffGroundRollLogger = new PerformanceCalculationLog.LogItem(
                $"Making takeoff ground roll adjustments for runway {runway.Runway_Name}");

            takeoffAdjustmentsLogger.SubItems.Add(takeoffGroundRollLogger);

            result.Takeoff_GroundRoll =
                Aircraft.MakeTakeoffAdjustments
                (
                    result,
                    IntepolatedTakeoffPerformanceData.DistanceGroundRoll.Value,
                    takeoffGroundRollLogger,
                    AirconOption
                );

            PerformanceCalculationLog.LogItem takeoff50FtClearanceLogger = new PerformanceCalculationLog.LogItem(
                $"Making takeoff 50 ft clearance adjustments for runway {runway.Runway_Name}");

            takeoffAdjustmentsLogger.SubItems.Add(takeoff50FtClearanceLogger);

            result.Takeoff_50FtClearance = Aircraft.MakeTakeoffAdjustments
                (
                    result,
                    IntepolatedTakeoffPerformanceData.DistanceToClear50Ft.Value,
                    takeoff50FtClearanceLogger,
                    AirconOption
                );
        }

        private void CalculateLandingPerformanceForRunway(Runway runway, PerformanceCalculationLog.LogItem runwayLogger, CalculationResultForRunway result)
        {
            PerformanceCalculationLog.LogItem landingAdjustmentsLogger = new PerformanceCalculationLog.LogItem(
                $"Making landing adjustments for runway {runway.Runway_Name}");

            runwayLogger.SubItems.Add(landingAdjustmentsLogger);

            PerformanceCalculationLog.LogItem landingGroundRollLogger = new PerformanceCalculationLog.LogItem(
                $"Making landing ground roll adjustments for runway {runway.Runway_Name}");

            landingAdjustmentsLogger.SubItems.Add(landingGroundRollLogger);

            result.Landing_GroundRoll = Aircraft.MakeLandingAdjustments
                (
                    result,
                    IntepolatedLandingPerformanceData.DistanceGroundRoll.Value,
                    landingGroundRollLogger,
                    AirconOption
                );

            PerformanceCalculationLog.LogItem landing50FtClearanceLogger = new PerformanceCalculationLog.LogItem(
                $"Making landing 50 ft clearance adjustments for runway {runway.Runway_Name}");

            landingAdjustmentsLogger.SubItems.Add(landing50FtClearanceLogger);

            result.Landing_50FtClearance = Aircraft.MakeLandingAdjustments
                (
                    result,
                    IntepolatedLandingPerformanceData.DistanceToClear50Ft.Value,
                    landing50FtClearanceLogger,
                    AirconOption
                );
        }

    }
}