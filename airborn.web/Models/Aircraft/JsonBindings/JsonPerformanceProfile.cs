using System;

namespace Airborn.web.Models
{


    // JsonPerformanceProfile has in the Json
    //   a Type (Takeoff or Landing)
    //   a PressureAltitude
    //   and binds to a set of JsonPerformanceProfileResults which each have a Temperature and a corresponding Distance
    // I know, it's weird
    public class JsonPerformanceProfile
    {
        public int PressureAltitude { get; set; }

        // this will be either Takeoff or Landing in the json
        public string Type { get; set; }

        public JsonPerformanceProfileResultList GroundRoll { get; set; }

        public JsonPerformanceProfileResultList Clear50FtObstacle { get; set; }

    }

}