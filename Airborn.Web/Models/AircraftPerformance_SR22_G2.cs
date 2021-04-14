using System;

namespace Airborn.web.Models
{
    public class AircraftPerformance_SR22_G2 : AircraftPerformanceBase
    {
       public AircraftPerformance_SR22_G2(AircraftPerformanceProfileList profiles, Scenario scenario)
        {
            _profiles = profiles;
            _scenario = scenario;
        }



        protected override double MakeTakeoffAdjustments(double takeoffDistance)
        {
            // subtract 10% for each 12 knots of headwind

            // add 10% for each 2 knots of tailwind up to 10 knots

            return 0;
        }
        protected override double MakeLandingAdjustments(double landingDistance)
        {
            return 0;
        }

    }
}