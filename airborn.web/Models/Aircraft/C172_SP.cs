using System;

namespace Airborn.web.Models
{
    public class C172_SP : Aircraft
    {

        public C172_SP()
        {
        }

        public override decimal MakeTakeoffAdjustments(PerformanceCalculationResult result, decimal takeoffDistance)
        {

            if (result.HeadwindComponent > 0)
            {
                // subtract 10% for each 9 knots of headwind
                takeoffDistance = takeoffDistance * (1 - ((result.HeadwindComponent / 9) * 0.1m));
            }
            else if (result.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                takeoffDistance = takeoffDistance * (1 - adjustment);

            }



            return takeoffDistance;
        }

        public override decimal MakeLandingAdjustments(PerformanceCalculationResult result, decimal landingDistance)
        {
            if (result.HeadwindComponent > 0)
            {
                // subtract 10% for each 9 knots of headwind
                landingDistance = landingDistance * (1 - ((result.HeadwindComponent / 9) * 0.1m));
            }
            else if (result.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                landingDistance = landingDistance * (1 - adjustment);

            }

            return landingDistance;
        }

        public override string JsonFileName_LowerWeight()
        {
            return "../Data/C172_SP_2200.json";
        }


        public override string JsonFileName_HigherWeight()
        {
            return "../Data/C172_SP_2550.json";
        }

        public override int GetLowerWeight()
        {
            return 2200;
        }

        public override int GetHigherWeight()
        {
            return 2550;
        }
    }
}