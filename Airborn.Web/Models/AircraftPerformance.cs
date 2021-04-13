using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public class AircraftPerformance
    {
        private AircraftPerformance()
        {
        }

        private AircraftPerformance(AircraftPerformanceProfileList profiles)
        {
            _profiles = profiles;
        }

        public enum ScenarioMode {
            Takeoff_GroundRoll,
            Takeoff_50FtClearance,
            Landing_GroundRoll,
            Landing_50FtClearance
        }

        private AircraftPerformanceProfileList _profiles;

        public AircraftPerformanceProfileList Profiles 
        {
            get
            {
                return _profiles;
            }
        }

        public static AircraftPerformance CreateFromJson()
        {
            

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            };

            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../SR22_G2_Takeoff.json");

            StreamReader sr = new StreamReader(filepath);

            var json = sr.ReadToEnd();

            AircraftPerformanceProfileList profiles = JsonConvert.DeserializeObject<AircraftPerformanceProfileList>(json);

            AircraftPerformance aircraftPerformance = new AircraftPerformance(profiles);

            return aircraftPerformance;
        }


        public class AircraftPerformanceProfile
        {
            public int PressureAltitude {get; set;}

            public AircraftPerformanceProfileResultList GroundRoll {get; set;}

            public AircraftPerformanceProfileResultList Clear50FtObstacle {get; set;}

        }

        public class AircraftPerformanceProfileList : List<AircraftPerformanceProfile>
        {
            public AircraftPerformanceProfile FindByPressureAltitude(int pressureAltitude)
            {
                return this.Find (p => p.PressureAltitude == pressureAltitude);
            }


            public int GetPerformanceData(ScenarioMode scenario, int pressureAltitude, int temperature)
            {
                switch(scenario)
                {
                    case ScenarioMode.Takeoff_50FtClearance:
                        return FindByPressureAltitude(pressureAltitude).Clear50FtObstacle.FindByTemperature(temperature);
                    case ScenarioMode.Takeoff_GroundRoll:
                        return FindByPressureAltitude(pressureAltitude).GroundRoll.FindByTemperature(temperature);                     

                }

                throw new ArgumentException("No performance data found for pressure altitude" + pressureAltitude + " and temperature " + temperature  );
            }
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

        public int CalculateTakeoffDistanceGroundRoll(Scenario scenario, ScenarioMode scenarioMode)
        {
            int lowerPressureAltidude = GetLowerBoundForInterpolation(scenario.PressureAltitude, 1000);
            int upperPressureAltidude = GetUpperBoundForInterpolation(scenario.PressureAltitude, 1000);
            int lowerTemperature = GetLowerBoundForInterpolation(scenario.TemperatureCelcius, 10);
            int upperTemperature = GetUpperBoundForInterpolation(scenario.TemperatureCelcius, 10);

            int distanceForLowerPressureAltitudeLowerTemp = Profiles.GetPerformanceData(scenarioMode, lowerPressureAltidude, lowerTemperature);
            int distanceForUpperPressureAltitudeLowerTemp = Profiles.GetPerformanceData(scenarioMode, upperPressureAltidude, lowerTemperature);
            
            double distanceLowerTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeLowerTemp,
                distanceForUpperPressureAltitudeLowerTemp,
                scenario.PressureAltitude,
                1000 // pressure altitudes in the json come in intervals of 1000
            );

            int distanceForLowerPressureAltitudeUpperTemp = Profiles.GetPerformanceData(scenarioMode, lowerPressureAltidude, upperTemperature);
            int distanceForUpperPressureAltitudeUpperTemp = Profiles.GetPerformanceData(scenarioMode, upperPressureAltidude, upperTemperature);

            double distanceUpperTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeUpperTemp,
                distanceForUpperPressureAltitudeUpperTemp,
                scenario.PressureAltitude,
                1000 // pressure altitudes in the json come in intervals of 1000
            );

            double distanceInterpolated = Interpolate(
                (int)distanceLowerTempInterpolated,
                (int)distanceUpperTempInterpolated,
                scenario.TemperatureCelcius,
                10 // temperatures in the json come in intervals of 10
            );

            return (int)distanceInterpolated;

        }

        public static double CalculateInterpolationFactor (int value, int x1, int x2)
        {
            if (value < x1) { throw new ArgumentOutOfRangeException(); }
            if (value > x2) { throw new ArgumentOutOfRangeException(); }

            double interpolationFactor = ((double)(value - x1)) / ((double)(x2 - x1));

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

        public static double Interpolate(int lowerValue, int upperValue, int valueForInterpolation, int desiredInterval)
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