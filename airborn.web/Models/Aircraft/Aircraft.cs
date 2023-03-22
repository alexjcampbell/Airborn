using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public abstract class Aircraft
    {

        public Aircraft()
        {
        }

       
        // the implementing aircraft class must implement the POH rules for adjusting
        // the performance data for wind, slope, surface etc
        public abstract double MakeTakeoffAdjustments(PerformanceCalculationResultForRunway result, double takeoffDistance);
        public abstract double MakeLandingAdjustments(PerformanceCalculationResultForRunway result, double landingDistance);

        public abstract string JsonFileName();



    }
}