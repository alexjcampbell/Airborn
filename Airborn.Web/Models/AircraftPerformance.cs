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

        public int CalculateTakeoffDistanceGroundRoll(Scenario scenario)
        {
            int lowerPressureAltidude = GetLowerBoundForInterpolation(scenario.PressureAltitude, 1000);
            int upperPressureAltidude = GetUpperBoundForInterpolation(scenario.PressureAltitude, 1000);
            int lowerTemperature = GetLowerBoundForInterpolation(scenario.TemperatureCelcius, 10);
            int upperTemperature = GetUpperBoundForInterpolation(scenario.TemperatureCelcius, 10);

            int distanceForLowerPressureAltitudeLowerTemp = Profiles.FindByPressureAltitude(lowerPressureAltidude).GroundRoll.FindByTemperature(lowerTemperature);
            int distanceForUpperPressureAltitudeLowerTemp = Profiles.FindByPressureAltitude(upperPressureAltidude).GroundRoll.FindByTemperature(lowerTemperature);
            double distanceLowerTempInterpolated = 
                (distanceForUpperPressureAltitudeLowerTemp - distanceForLowerPressureAltitudeLowerTemp) 
                * 
                CalculateInterpolationFactor(scenario.PressureAltitude, lowerPressureAltidude, upperPressureAltidude)
                +
                distanceForLowerPressureAltitudeLowerTemp
                ;

            int distanceForLowerPressureAltitudeUpperTemp = Profiles.FindByPressureAltitude(lowerPressureAltidude).GroundRoll.FindByTemperature(upperTemperature);
            int distanceForUpperPressureAltitudeUpperTemp = Profiles.FindByPressureAltitude(upperPressureAltidude).GroundRoll.FindByTemperature(upperTemperature);

            double distanceUpperTempInterpolated = 
                (distanceForUpperPressureAltitudeUpperTemp - distanceForLowerPressureAltitudeUpperTemp) 
                * 
                CalculateInterpolationFactor(scenario.PressureAltitude, lowerPressureAltidude, upperPressureAltidude)
                +
                distanceForLowerPressureAltitudeUpperTemp
                ;

            double distanceInterpolated =
                (distanceUpperTempInterpolated - distanceLowerTempInterpolated)
                *
                CalculateInterpolationFactor(scenario.TemperatureCelcius, lowerTemperature, upperTemperature)
                +
                distanceLowerTempInterpolated
                ;

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
    }
}