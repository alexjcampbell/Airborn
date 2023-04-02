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

        private int _temperatureCelcius;

        public int TemperatureCelcius
        {
            get
            {
                return _temperatureCelcius;
            }
            set
            {
                _temperatureCelcius = value;
            }
        }


        private int _qnh;

        public int QNH
        {
            get
            {
                return _qnh;
            }
            set
            {
                _qnh = value;
            }
        }

        public int PressureAltitude
        {
            get
            {
                return ((1013 - QNH) * 30) + Airport.FieldElevation;
            }
        }

        private double _aircraftWeight;

        public double AircraftWeight
        {
            get
            {
                return _aircraftWeight;
            }
            set
            {
                _aircraftWeight = value;
            }
        }

        public double DensityAltitude
        {
            get
            {
                return ((TemperatureCelcius - ISATemperature) * 120) + PressureAltitude;
            }
        }

        public double ISATemperature
        {
            get
            {
                return 15 - (2 * ((double)PressureAltitude / 1000));
            }
        }


        public void Calculate(AirportDbContext db)
        {


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
        /// Gets the raw distance from the JSON file for a given scenario mode
        /// </summary>
        public decimal GetDistanceFromJson(ScenarioMode scenarioMode, JsonFile jsonFile)
        {
            /*  The performance data in the JSON is provided by the manufacturer in 
                pressure altitude intervals of 1000 ft and temperature intervals of
                10 degrees celcius. This function looks bit complicated but all it 
                is getting the two relevant performance numbers and interpolating between
                them

                i.e. if the scenario's temperature is 12 degrees and pressure altitude
                is 1500 ft, it will find the performance data for 10 and 20 degrees C
                and pressure altitude of 1000 and 2000 ft, and interpolate between them
            */

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

            // get the upper and lower bounds for pressure altitude and temperature
            // ie using the example below this will return 1000 ft and 2000 ft
            // and 10 and 20 degreses
            int lowerPressureAltidude = GetLowerBoundForInterpolation(PressureAltitude, 1000);
            int upperPressureAltidude = GetUpperBoundForInterpolation(PressureAltitude, 1000);
            int lowerTemperature = GetLowerBoundForInterpolation(TemperatureCelcius, 10);
            int upperTemperature = GetUpperBoundForInterpolation(TemperatureCelcius, 10);

            // then get the performance data for the lower temperature and the lower and higher
            // pressure altitude
            decimal distanceForLowerPressureAltitudeLowerTemp = jsonFile.GetPerformanceDataValueForConditions(profileList, this, scenarioMode, lowerPressureAltidude, lowerTemperature);
            decimal distanceForUpperPressureAltitudeLowerTemp = jsonFile.GetPerformanceDataValueForConditions(profileList, this, scenarioMode, upperPressureAltidude, lowerTemperature);

            // interpolate between the lower and higher pressure altitude for the lower temperature
            decimal distanceLowerTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeLowerTemp,
                distanceForUpperPressureAltitudeLowerTemp,
                PressureAltitude,
                1000 // pressure altitudes in the json comes in intervals of 1000
            );

            // then get the performance data for the higher temperature and the lower and higher
            // pressure altitude
            decimal distanceForLowerPressureAltitudeUpperTemp = jsonFile.GetPerformanceDataValueForConditions(profileList, this, scenarioMode, lowerPressureAltidude, upperTemperature);
            decimal distanceForUpperPressureAltitudeUpperTemp = jsonFile.GetPerformanceDataValueForConditions(profileList, this, scenarioMode, upperPressureAltidude, upperTemperature);

            // interpolate between the lower and higher pressure altitude for the higher temperature
            decimal distanceUpperTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeUpperTemp,
                distanceForUpperPressureAltitudeUpperTemp,
                PressureAltitude,
                1000 // pressure altitudes in the json comes in intervals of 1000
            );

            // interpolate between the lower and higher temperature
            decimal distanceInterpolated = Interpolate(
                (int)distanceLowerTempInterpolated,
                (int)distanceUpperTempInterpolated,
                TemperatureCelcius,
                10 // temperatures in the json comes in intervals of 10
            );

            System.Diagnostics.Debug.Write(
                $"Distance interpolated: {distanceInterpolated} for pressure altitude {PressureAltitude} and temperature {TemperatureCelcius} ");

            return distanceInterpolated;


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

    }
}