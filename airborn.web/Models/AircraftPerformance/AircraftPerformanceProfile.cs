using System;

namespace Airborn.web.Models{


        public class AircraftPerformanceProfile
        {
            public int PressureAltitude {get; set;}

            // this will be either Takeoff or Landing in the json
            public string Type { get; set; }

            public AircraftPerformanceProfileResultList GroundRoll {get; set;}

            public AircraftPerformanceProfileResultList Clear50FtObstacle {get; set;}

        }

}