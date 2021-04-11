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

        private AircraftPerformance(List<AircraftPerformanceProfile> profiles)
        {
            _profiles = profiles;
        }

        private List<AircraftPerformanceProfile> _profiles;

        public List<AircraftPerformanceProfile> Profiles 
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

            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../SR22.json");

            StreamReader sr = new StreamReader(filepath);

            var json = sr.ReadToEnd();

            List<AircraftPerformanceProfile> profiles = JsonConvert.DeserializeObject<List<AircraftPerformanceProfile>>(json);

            AircraftPerformance aircraftPerformance = new AircraftPerformance(profiles);

            return aircraftPerformance;
        }

        public class AircraftPerformanceProfile
        {
            public int PressureAltitude {get; set;}

            public IList<AircraftPerformanceProfileResult> GroundRoll {get; set;}
        }

        public class AircraftPerformanceProfileResult{
            public int Temperature {get;set;}
            public int Distance {get;set;}
        }
    }
}