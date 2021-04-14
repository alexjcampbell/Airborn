using System;
using System.IO;
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

        public double? Takeoff_GroundRoll
        { 
            get {
                return MakeTakeoffAdjustments(GetInterpolatedDistanceFromJson(ScenarioMode.Takeoff_GroundRoll));
            } 
         }
            
        public double? Takeoff_50FtClearance
        { 
            get {
                return MakeTakeoffAdjustments(GetInterpolatedDistanceFromJson(ScenarioMode.Takeoff_50FtClearance));
            } 
         }

        public double? Landing_GroundRoll
        { 
            get {
                return MakeLandingAdjustments(GetInterpolatedDistanceFromJson(ScenarioMode.Landing_GroundRoll));
            } 
         }

        public double? Landing_50FtClearance
        { 
            get {
                return MakeLandingAdjustments(GetInterpolatedDistanceFromJson(ScenarioMode.Landing_50FtClearance));
            } 
         }

        public static AircraftPerformanceBase CreateFromJson(Scenario scenario)
        {
            

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            };

            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../SR22_G2.json");

            StreamReader sr = new StreamReader(filepath);

            var json = sr.ReadToEnd();

            AircraftPerformanceProfileList profiles = JsonConvert.DeserializeObject<AircraftPerformanceProfileList>(json);

            AircraftPerformanceBase aircraftPerformance = new AircraftPerformance_SR22_G2(profiles, scenario);

            return aircraftPerformance;
        }


        public class AircraftPerformanceProfile
        {
            public int PressureAltitude {get; set;}

            // this will be either Takeoff or Landing in the json
            public string Type { get; set; }

            public AircraftPerformanceProfileResultList GroundRoll {get; set;}

            public AircraftPerformanceProfileResultList Clear50FtObstacle {get; set;}

        }

        public class AircraftPerformanceProfileList : List<AircraftPerformanceProfile>
        {
            public AircraftPerformanceProfile FindByPressureAltitude(ScenarioMode scenarioMode, int pressureAltitude)
            {
                string type = "";

                switch (scenarioMode) {
                    case ScenarioMode.Takeoff_50FtClearance:
                    case ScenarioMode.Takeoff_GroundRoll:
                        type = "Takeoff";
                        break;
                    case ScenarioMode.Landing_50FtClearance:
                    case ScenarioMode.Landing_GroundRoll:
                        type = "Landing";
                        break;                        
                }

                return this.Find (
                    p => 
                    p.PressureAltitude == pressureAltitude
                    &
                    p.Type == type
                    );
            }


            public double GetPerformanceData(ScenarioMode scenarioMode, int pressureAltitude, int temperatureCelcius)
            {
                switch(scenarioMode)
                {
                    case ScenarioMode.Takeoff_50FtClearance:
                        return FindByPressureAltitude(ScenarioMode.Takeoff_50FtClearance, pressureAltitude).Clear50FtObstacle.FindByTemperature(temperatureCelcius);
                    case ScenarioMode.Takeoff_GroundRoll:
                        return FindByPressureAltitude(ScenarioMode.Takeoff_GroundRoll, pressureAltitude).GroundRoll.FindByTemperature(temperatureCelcius);                     
                    case ScenarioMode.Landing_50FtClearance:
                        return FindByPressureAltitude(ScenarioMode.Landing_50FtClearance, pressureAltitude).Clear50FtObstacle.FindByTemperature(temperatureCelcius);
                    case ScenarioMode.Landing_GroundRoll:
                        return FindByPressureAltitude(ScenarioMode.Landing_GroundRoll, pressureAltitude).GroundRoll.FindByTemperature(temperatureCelcius);                     


                }

                throw new ArgumentException("No performance data found for pressure altitude: " + pressureAltitude + " and temperature: " + temperatureCelcius);
            }
        }

        public double GetInterpolatedDistanceFromJson(ScenarioMode scenarioMode)
        {

            int lowerPressureAltidude = GetLowerBoundForInterpolation(Scenario.PressureAltitude, 1000);
            int upperPressureAltidude = GetUpperBoundForInterpolation(Scenario.PressureAltitude, 1000);
            int lowerTemperature = GetLowerBoundForInterpolation(Scenario.TemperatureCelcius, 10);
            int upperTemperature = GetUpperBoundForInterpolation(Scenario.TemperatureCelcius, 10);

            double distanceForLowerPressureAltitudeLowerTemp = Profiles.GetPerformanceData(scenarioMode, lowerPressureAltidude, lowerTemperature);
            double distanceForUpperPressureAltitudeLowerTemp = Profiles.GetPerformanceData(scenarioMode, upperPressureAltidude, lowerTemperature);
            
            double distanceLowerTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeLowerTemp,
                distanceForUpperPressureAltitudeLowerTemp,
                Scenario.PressureAltitude,
                1000 // pressure altitudes in the json come in intervals of 1000
            );

            double distanceForLowerPressureAltitudeUpperTemp = Profiles.GetPerformanceData(scenarioMode, lowerPressureAltidude, upperTemperature);
            double distanceForUpperPressureAltitudeUpperTemp = Profiles.GetPerformanceData(scenarioMode, upperPressureAltidude, upperTemperature);

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


        public class AircraftPerformanceProfileResult{
            public int Temperature {get;set;}
            public int Distance {get;set;}
        }

        public class AircraftPerformanceProfileResultList : List<AircraftPerformanceProfileResult>
        {
            public int FindByTemperature(int temperature)
            {
                AircraftPerformanceProfileResult result = this.Find (t => t.Temperature == temperature);

                return result.Distance;
            }
        }        
    }
}