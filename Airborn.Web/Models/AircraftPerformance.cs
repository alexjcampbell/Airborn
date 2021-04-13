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

        public static double CalculateInterpolationFactor (double value, double x1, double x2)
        {
            if (value < x1) { throw new ArgumentOutOfRangeException(); }
            if (value > x2) { throw new ArgumentOutOfRangeException(); }

            return (value - x1) / (x2 - x1);
        }
    }
}