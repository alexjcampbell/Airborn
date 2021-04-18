using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public abstract class AircraftPerformanceBase
    {
        protected AircraftPerformanceBase()
        {
        }

        public AircraftPerformanceBase(Scenario scenario, string path)
        {
            _scenario = scenario;
            LoadJson(path);
        }


        protected Scenario _scenario;

        public Scenario Scenario
        {
            get
            {
                return _scenario;
            }
        }

        public enum AircraftType
        {
            Cirrus_SR22_G2
        }

        protected AircraftPerformanceProfileList _profiles;

        public AircraftPerformanceProfileList Profiles
        {
            get
            {
                return _profiles;
            }
            set
            {
                _profiles = value;
            }
        }

        private double _takeoff_GroundRoll;

        public double? Takeoff_GroundRoll
        {
            get
            {
                return _takeoff_GroundRoll;
            }
        }

        private double _takeoff_50FtClearance;

        public double? Takeoff_50FtClearance
        {
            get
            {
                return _takeoff_50FtClearance;
            }
        }

        private double _landing_Groundroll;

        public double? Landing_GroundRoll
        {
            get
            {
                return _landing_Groundroll;
            }
        }

        private double _landing_50FtClearance;

        public double? Landing_50FtClearance
        {
            get
            {
                return _landing_50FtClearance;
            }
        }

        public void LoadJson(string path)
        {


            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            };

            StreamReader sr = new StreamReader(path);

            var json = sr.ReadToEnd();

            JsonConvert.PopulateObject(json, this); ;

            InitialiseTakeoffAndLandingCalculations();
        }


        public void InitialiseTakeoffAndLandingCalculations()
        {
            _takeoff_GroundRoll = MakeTakeoffAdjustments(
                GetInterpolatedDistanceFromJson(ScenarioMode.Takeoff_GroundRoll)
                );

            _takeoff_50FtClearance = MakeTakeoffAdjustments(
                GetInterpolatedDistanceFromJson(ScenarioMode.Takeoff_50FtClearance)
                );

            _landing_Groundroll = MakeLandingAdjustments(
                GetInterpolatedDistanceFromJson(ScenarioMode.Landing_GroundRoll)
                );

            _landing_50FtClearance = MakeLandingAdjustments(
                    GetInterpolatedDistanceFromJson(ScenarioMode.Landing_50FtClearance)
                );

        }

        public double GetInterpolatedDistanceFromJson(ScenarioMode scenarioMode)
        {
            /*  The performance data in the JSON is provided by the manufacturer in 
                pressure altitude intervals of 1000 ft and temperature intervals of
                10 degrees celcius. This function looks bit complicated but all it 
                is doing is interpolating between the two

                i.e. if the scenario's temperature is 12 degrees and pressure altitude
                is 1500 ft, it will find the performance data for 10 and 20 degrees C
                and pressure altitude of 1000 and 2000 ft, and interpolate between them
            */

            // get the upper and lower bounds for pressure altitude and temperature
            // ie using the example below this will return 1000 ft and 2000 ft
            // and 10 and 20 degreses
            int lowerPressureAltidude = GetLowerBoundForInterpolation(Scenario.PressureAltitude, 1000);
            int upperPressureAltidude = GetUpperBoundForInterpolation(Scenario.PressureAltitude, 1000);
            int lowerTemperature = GetLowerBoundForInterpolation(Scenario.TemperatureCelcius, 10);
            int upperTemperature = GetUpperBoundForInterpolation(Scenario.TemperatureCelcius, 10);

            // then get the performance data for the lower temperature and the lower and higher
            // pressure altitude
            double distanceForLowerPressureAltitudeLowerTemp = Profiles.GetPerformanceDataValueForConditions(Scenario, scenarioMode, lowerPressureAltidude, lowerTemperature);
            double distanceForUpperPressureAltitudeLowerTemp = Profiles.GetPerformanceDataValueForConditions(Scenario, scenarioMode, upperPressureAltidude, lowerTemperature);

            // interpolate between the lower and higher pressure altitude for the lower temperature
            double distanceLowerTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeLowerTemp,
                distanceForUpperPressureAltitudeLowerTemp,
                Scenario.PressureAltitude,
                1000 // pressure altitudes in the json comes in intervals of 1000
            );

            // then get the performance data for the higher temperature and the lower and higher
            // pressure altitude
            double distanceForLowerPressureAltitudeUpperTemp = Profiles.GetPerformanceDataValueForConditions(Scenario, scenarioMode, lowerPressureAltidude, upperTemperature);
            double distanceForUpperPressureAltitudeUpperTemp = Profiles.GetPerformanceDataValueForConditions(Scenario, scenarioMode, upperPressureAltidude, upperTemperature);

            // interpolate between the lower and higher pressure altitude for the higher temperature
            double distanceUpperTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeUpperTemp,
                distanceForUpperPressureAltitudeUpperTemp,
                Scenario.PressureAltitude,
                1000 // pressure altitudes in the json comes in intervals of 1000
            );

            // interpolate between the lower and higher temperature
            double distanceInterpolated = Interpolate(
                (int)distanceLowerTempInterpolated,
                (int)distanceUpperTempInterpolated,
                Scenario.TemperatureCelcius,
                10 // temperatures in the json comes in intervals of 10
            );

            System.Diagnostics.Debug.Write(
                $"Distance interpolated: {distanceInterpolated} for pressure altitude {Scenario.PressureAltitude} and temperature {Scenario.TemperatureCelcius} ");

            return distanceInterpolated;

        }

        // the implementing class must implement the POH rules for adjusting
        // the performance data for wind, slope, surface etc
        public abstract double MakeTakeoffAdjustments(double takeoffDistance);
        public abstract double MakeLandingAdjustments(double landingDistance);

        public static double CalculateInterpolationFactor(int value, int x1, int x2)
        {
            if (value < x1) { throw new ArgumentOutOfRangeException(); }
            if (value > x2) { throw new ArgumentOutOfRangeException(); }

            double interpolationFactor = (double)(value - x1) / (double)(x2 - x1);

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

        public static double Interpolate(double lowerValue, double upperValue, int valueForInterpolation, int desiredInterval)
        {
            int lowerInterpolation = GetLowerBoundForInterpolation(valueForInterpolation, desiredInterval);
            int upperInterpolation = GetUpperBoundForInterpolation(valueForInterpolation, desiredInterval);

            double interpolationFactor = CalculateInterpolationFactor(valueForInterpolation, lowerInterpolation, upperInterpolation);

            double interpolatedValue =
                (upperValue - lowerValue)
                *
                interpolationFactor
                + lowerValue;

            return interpolatedValue;

        }


    }
}