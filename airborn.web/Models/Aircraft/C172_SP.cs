using System;

namespace Airborn.web.Models
{
    public class C172_SP : Aircraft
    {

        public C172_SP()
        {
        }

        public override Distance MakeTakeoffAdjustments(PerformanceCalculationResultForRunway result, Distance unadjustedTakeoffDistance)
        {
            decimal adjustedTakeoffDistance = unadjustedTakeoffDistance.TotalFeet;

            if (result.HeadwindComponent > 0)
            {
                // subtract 10% for each 9 knots of headwind
                adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - ((result.HeadwindComponent / 9) * 0.1m));
            }
            else if (result.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - adjustment);

            }

            return Distance.FromFeet(adjustedTakeoffDistance);
        }

        public override Distance MakeLandingAdjustments(PerformanceCalculationResultForRunway result, Distance unadjustedLandingDistance)
        {
            decimal adjustedLandingDistance = unadjustedLandingDistance.TotalFeet;

            if (result.HeadwindComponent > 0)
            {
                // subtract 10% for each 9 knots of headwind
                adjustedLandingDistance = adjustedLandingDistance * (1 - ((result.HeadwindComponent / 9) * 0.1m));
            }
            else if (result.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                // make the adjustment
                adjustedLandingDistance = adjustedLandingDistance * (1 - adjustment);

            }

            return Distance.FromFeet(adjustedLandingDistance);
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