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
        protected Aircraft()
        {
        }

        public Aircraft(Scenario scenario)
        {
            _scenario = scenario;
        }


        protected Scenario _scenario;

        public Scenario Scenario
        {
            get
            {
                return _scenario;
            }
        }

       
        // the implementing aircraft class must implement the POH rules for adjusting
        // the performance data for wind, slope, surface etc
        public abstract double MakeTakeoffAdjustments(double takeoffDistance);
        public abstract double MakeLandingAdjustments(double landingDistance);


        
    }
}