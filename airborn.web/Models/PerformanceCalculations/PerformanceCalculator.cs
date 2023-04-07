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
            AircraftType aircraftType,
            Airport airport,
            Wind wind,
            string path)
        {
            AircraftType = aircraftType;
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

        public double AircraftWeight
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

        // notes to return to the UI about the calculations
        public List<string> Notes = new List<string>();

        public void Calculate(AirportDbContext db)
        {

            if(PressureAltitude < 0)
            {
                Notes.Add("Pressure altitude is negative. The POH only provides performance data for positive pressure altitudes, so we'll calculate based on a pressure altitude of 0 ft (sea level). Actual performance should be better than the numbers stated here.");
            }

            if(TemperatureCelcius < 0)
            {
                Notes.Add("Temperature is negative. The POH data only provides performance date for positive temperatures, so we'll calculate based on a temperature of zero degrees C. Actual performance should be better than the numbers stated here.");
            }            

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(AircraftType);

            string fullPathLowerWeight = Path.Combine(JsonPath, aircraft.JsonFileName_LowerWeight());
            string fullPathHigherWeight = Path.Combine(JsonPath, aircraft.JsonFileName_HigherWeight());

            JsonFile jsonFileLowerWeight = new JsonFile(fullPathLowerWeight);
            JsonFile jsonFileHigherWeight = new JsonFile(fullPathHigherWeight);

            List<PerformanceCalculationResult> results = new List<PerformanceCalculationResult>();

            foreach (Runway runway in
                db.Runways.Where(runway => runway.Airport_Ident == Airport.Ident.ToUpper()).ToList<Runway>())
            {
                Runway primaryRunway = runway;

                // RunwayIdentifer could be 10R or 28L so we use Substring to get only the first two characters
                string runwayName = Regex.Replace(runway.Runway_Name, @"\D+", "");

                primaryRunway.RunwayHeading =
                    new Direction(
                        int.Parse(runwayName) * 10,
                        0
                        );

                Runways.Add(primaryRunway);
            }



            foreach (Runway runway in Runways)
            {
                PerformanceCalculationResult result = CalculatePerformanceForRunway(aircraft, jsonFileLowerWeight, jsonFileHigherWeight, runway);

                Results.Add(result);
            }

        }

        /// <summary>
        /// Calculates the performance for a given runway
        /// </summary>
        private PerformanceCalculationResult CalculatePerformanceForRunway(Aircraft aircraft, JsonFile jsonFileLowerWeight, JsonFile jsonFileHigherWeight, Runway runway)
        {
            PerformanceCalculationResult result =
                new PerformanceCalculationResult(runway, Wind);

            result.JsonFileLowerWeight = jsonFileLowerWeight;
            result.JsonFileHigherWeight = jsonFileHigherWeight;

            decimal weightInterpolationFactor =
                aircraft.GetWeightInterpolationFactor(Convert.ToInt32(AircraftWeight));

            result.Takeoff_GroundRoll =
                aircraft.MakeTakeoffAdjustments
                (
                    result,
                    InterpolateDistanceByWeight(
                        weightInterpolationFactor,
                        GetDistanceFromJson(ScenarioMode.Takeoff_GroundRoll, jsonFileLowerWeight),
                        GetDistanceFromJson(ScenarioMode.Takeoff_GroundRoll, jsonFileLowerWeight)
                ));

            result.Takeoff_50FtClearance = aircraft.MakeTakeoffAdjustments
                (
                    result,
                    InterpolateDistanceByWeight(
                        weightInterpolationFactor,
                        GetDistanceFromJson(ScenarioMode.Takeoff_50FtClearance, jsonFileLowerWeight),
                        GetDistanceFromJson(ScenarioMode.Takeoff_50FtClearance, jsonFileHigherWeight)
                ));

            result.Landing_GroundRoll = aircraft.MakeLandingAdjustments
                (
                    result,
                    InterpolateDistanceByWeight(
                        weightInterpolationFactor,
                        GetDistanceFromJson(ScenarioMode.Landing_GroundRoll, jsonFileLowerWeight),
                        GetDistanceFromJson(ScenarioMode.Landing_GroundRoll, jsonFileHigherWeight)
                ));

            result.Landing_50FtClearance = aircraft.MakeLandingAdjustments
                (
                    result,
                    InterpolateDistanceByWeight(
                        weightInterpolationFactor,
                        GetDistanceFromJson(ScenarioMode.Landing_50FtClearance, jsonFileLowerWeight),
                        GetDistanceFromJson(ScenarioMode.Landing_50FtClearance, jsonFileHigherWeight)
                ));

            return result;
        }

        /// <summary>
        /// Interpolates between two distances based on the weight interpolation factor
        /// i.e. if the weight interpolation factor is 0.5, it will return the average of the two distances
        /// </summary>
        private static decimal InterpolateDistanceByWeight(decimal weightInterpolationFactor, decimal distance_LowerWeight, decimal distance_HigherWeight)
        {
            return distance_LowerWeight +
                (weightInterpolationFactor * (distance_HigherWeight - distance_LowerWeight));
        }

        /// <summary>
        /// The performance data in the JSON is provided by the manufacturer in 
        /// pressure altitude intervals of 1000 ft and temperature intervals of
        /// 10 degrees celcius. This function looks bit complicated but all it 
        /// is getting the two relevant performance numbers and interpolating between
        /// them
        ///
        /// i.e. if the scenario's temperature is 12 degrees and pressure altitude
        /// is 1500 ft, it will find the performance data for 10 and 20 degrees C
        /// and pressure altitude of 1000 and 2000 ft, and interpolate between them
        /// </summary>
        public decimal GetDistanceFromJson(ScenarioMode scenarioMode, JsonFile jsonFile)
        {

            // gets the takeoff (or landing) profiles, depending on ScenarioMode
            JsonPerformanceProfileList profileList = PopulateJsonPerformanceProfileList(scenarioMode, jsonFile);

            int pressureAltitudeAsInt = (int)PressureAltitudeAlwaysPositiveOrZero;
            int temperatureAsInt = TemperatureCelcius > 0 ? (int)TemperatureCelcius : 0;

            const int pressureAltitudeInterval = 1000; // the interval at which performance data is provided in the POH
            const int temperatureInterval = 10; // the internal at which which temperature data is provided in the POH

            // get the upper and lower bounds for pressure altitude and temperature
            // ie using the example below this will return 1000 ft and 2000 ft
            // and 10 and 20 degreses
            int lowerPressureAltidude = GetLowerBoundForInterpolation(pressureAltitudeAsInt, pressureAltitudeInterval);
            int upperPressureAltidude = GetUpperBoundForInterpolation(pressureAltitudeAsInt, pressureAltitudeInterval);
            int lowerTemperature = GetLowerBoundForInterpolation(temperatureAsInt, temperatureInterval);
            int upperTemperature = GetUpperBoundForInterpolation(temperatureAsInt, temperatureInterval);

            // interpolate between the lower and higher pressure altitude for the lower temperature
            decimal distanceLowerTempInterpolated = Interpolate(
                jsonFile.GetPerformanceDataValueForConditions(profileList, scenarioMode, PressureAltitude, lowerPressureAltidude, lowerTemperature),
                jsonFile.GetPerformanceDataValueForConditions(profileList, scenarioMode, PressureAltitude, upperPressureAltidude, lowerTemperature),
                pressureAltitudeAsInt,
                pressureAltitudeInterval // pressure altitudes in the json comes in intervals of 1000
            );

            // interpolate between the lower and higher pressure altitude for the higher temperature
            decimal distanceUpperTempInterpolated = Interpolate(
                jsonFile.GetPerformanceDataValueForConditions(profileList, scenarioMode, PressureAltitude, lowerPressureAltidude, upperTemperature),
                jsonFile.GetPerformanceDataValueForConditions(profileList, scenarioMode, PressureAltitude, upperPressureAltidude, upperTemperature),
                pressureAltitudeAsInt,
                pressureAltitudeInterval // pressure altitudes in the json comes in intervals of 1000
            );

            // interpolate between the lower and higher temperature
            decimal distanceInterpolated = Interpolate(
                (int)distanceLowerTempInterpolated,
                (int)distanceUpperTempInterpolated,
                temperatureAsInt,
                10 // temperatures in the json comes in intervals of 10
            );

            return distanceInterpolated;


        }


        private static JsonPerformanceProfileList PopulateJsonPerformanceProfileList(ScenarioMode scenarioMode, JsonFile jsonFile)
        {
            JsonPerformanceProfileList profileList;

            if (scenarioMode == ScenarioMode.Takeoff_GroundRoll || scenarioMode == ScenarioMode.Takeoff_50FtClearance)
            {
                profileList = jsonFile.TakeoffProfiles;
            }
            else if (scenarioMode == ScenarioMode.Landing_GroundRoll || scenarioMode == ScenarioMode.Landing_50FtClearance)
            {
                profileList = jsonFile.LandingProfiles;
            }
            else
            {
                throw new Exception("Invalid scenario mode");
            }

            return profileList;
        }

        /// <summary>
        /// This function gives you an interpolation factor between two values:
        /// If you give it a value of 5 and the lower and upper bounds are 0 and 10,
        /// it will return 0.5
        /// </summary>
        public static decimal CalculateInterpolationFactor(int value, int lowerBound, int upperBound)
        {
            if (value < 0)
            {
                throw new PressureAltitudePerformanceProfileNotFoundException(value);
            }

            if (value < lowerBound) { throw new ArgumentOutOfRangeException(); }
            if (value > upperBound) { throw new ArgumentOutOfRangeException(); }

            decimal interpolationFactor = (decimal)(value - lowerBound) / (decimal)(upperBound - lowerBound);

            return interpolationFactor;
        }

        public static int GetLowerBoundForInterpolation(int value, int desiredInterval)
        {
            int lowerBound = value - (value % desiredInterval);

            return lowerBound;
        }

        public static int GetUpperBoundForInterpolation(int value, int desiredInterval)
        {
            int lowerBound = value - (value % desiredInterval);
            int upperBound = lowerBound + desiredInterval;

            return upperBound;
        }

        public static (int, int) GetUpperAndLowBoundsForInterpolation(int value, int desiredInterval)
        {
            return (GetLowerBoundForInterpolation(value, desiredInterval), GetUpperBoundForInterpolation(value, desiredInterval));
        }

        public static decimal Interpolate(decimal lowerValue, decimal upperValue, int valueForInterpolation, int desiredInterval)
        {
            int lowerInterpolation = GetLowerBoundForInterpolation(valueForInterpolation, desiredInterval);
            int upperInterpolation = GetUpperBoundForInterpolation(valueForInterpolation, desiredInterval);

            decimal interpolationFactor = CalculateInterpolationFactor(valueForInterpolation, lowerInterpolation, upperInterpolation);

            decimal interpolatedValue =
                (upperValue - lowerValue)
                *
                interpolationFactor
                + lowerValue;

            return interpolatedValue;

        }


        private struct PerformanceInputMatrix
        {
            public int TemperatureLowerValue
            {
                get;
                set;
            }

            public int TemperatureUpperValue
            {
                get;
                set;
            }

            public int PressureAltitudeLowerValue
            {
                get;
                set;
            }

            public int PressureAltitudeUpperValue
            {
                get;
                set;
            }

        }

    }
}