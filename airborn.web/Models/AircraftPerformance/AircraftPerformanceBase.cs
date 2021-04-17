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

        protected AircraftPerformanceBase(AircraftPerformanceProfileList profiles, Scenario scenario)
        {
            _profiles = profiles;
            _scenario = scenario;
        }


        protected Scenario _scenario;

        public Scenario Scenario{
            get {
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
        }

        private double _takeoff_GroundRoll;

        public double? Takeoff_GroundRoll
        { 
            get {
                return _takeoff_GroundRoll;
            } 
         }
            
        private double _takeoff_50FtClearance;

        public double? Takeoff_50FtClearance
        { 
            get {
                return _takeoff_50FtClearance;
            } 
         }

         private double _landing_Groundroll;

        public double? Landing_GroundRoll
        { 
            get {
                return _landing_Groundroll;
            } 
         }

        private double _landing_50FtClearance;

        public double? Landing_50FtClearance
        { 
            get {
                return _landing_50FtClearance;
            } 
         }

        public static AircraftPerformanceBase CreateFromJson(Scenario scenario, string path)
        {
            

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            };

            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            StreamReader sr = new StreamReader(filepath);

            var json = sr.ReadToEnd();

            AircraftPerformanceProfileList profiles = JsonConvert.DeserializeObject<AircraftPerformanceProfileList>(json);

            AircraftPerformanceBase aircraftPerformance = new AircraftPerformance_SR22_G2(profiles, scenario);

            aircraftPerformance.Initialise();

            return aircraftPerformance;
        }


        public void Initialise()
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

            int lowerPressureAltidude = GetLowerBoundForInterpolation(Scenario.PressureAltitude, 1000);
            int upperPressureAltidude = GetUpperBoundForInterpolation(Scenario.PressureAltitude, 1000);
            int lowerTemperature = GetLowerBoundForInterpolation(Scenario.TemperatureCelcius, 10);
            int upperTemperature = GetUpperBoundForInterpolation(Scenario.TemperatureCelcius, 10);

            double distanceForLowerPressureAltitudeLowerTemp = Profiles.GetPerformanceData(Scenario, scenarioMode, lowerPressureAltidude, lowerTemperature);
            double distanceForUpperPressureAltitudeLowerTemp = Profiles.GetPerformanceData(Scenario, scenarioMode, upperPressureAltidude, lowerTemperature);
            
            double distanceLowerTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeLowerTemp,
                distanceForUpperPressureAltitudeLowerTemp,
                Scenario.PressureAltitude,
                1000 // pressure altitudes in the json come in intervals of 1000
            );

            double distanceForLowerPressureAltitudeUpperTemp = Profiles.GetPerformanceData(Scenario,scenarioMode, lowerPressureAltidude, upperTemperature);
            double distanceForUpperPressureAltitudeUpperTemp = Profiles.GetPerformanceData(Scenario,scenarioMode, upperPressureAltidude, upperTemperature);

            double distanceUpperTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeUpperTemp,
                distanceForUpperPressureAltitudeUpperTemp,
                Scenario.PressureAltitude,
                1000 // pressure altitudes in the json come in intervals of 1000
            );

            double distanceInterpolated = Interpolate(
                (int)distanceLowerTempInterpolated,
                (int)distanceUpperTempInterpolated,
                Scenario.TemperatureCelcius,
                10 // temperatures in the json come in intervals of 10
            );

            System.Diagnostics.Debug.Write(
                $"Distance interpolated: {distanceInterpolated} for pressure altitude {Scenario.PressureAltitude} and temperature {Scenario.TemperatureCelcius} " );

            return distanceInterpolated;

        }

        public abstract double MakeTakeoffAdjustments(double takeoffDistance);
        public abstract double MakeLandingAdjustments(double landingDistance);

        public static double CalculateInterpolationFactor (int value, int x1, int x2)
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