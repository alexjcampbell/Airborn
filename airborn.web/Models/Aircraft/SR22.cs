using System;

namespace Airborn.web.Models
{
    public abstract class SR22 : Aircraft
    {

        public SR22()
        {
        }


        public override decimal MakeTakeoffAdjustments(PerformanceCalculationResult result, decimal takeoffDistance)
        {

            if (result.HeadwindComponent > 0)
            {
                // subtract 10% for each 12 knots of headwind
                takeoffDistance = takeoffDistance * (1- ((result.HeadwindComponent / 12) * 0.1m));
            }
            else if (result.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                if (adjustment > 0.5m) { adjustment = 0.5m; }

                takeoffDistance = takeoffDistance * (1 - adjustment);

            }



            return takeoffDistance;
        }

        public override decimal MakeLandingAdjustments(PerformanceCalculationResult result, decimal landingDistance)
        {
            if (result.HeadwindComponent > 0)
            {
                // subtract 10% for each 13 knots of headwind
                landingDistance = landingDistance * (1 - ((result.HeadwindComponent / 13) * 0.1m));
            }
            else if (result.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                if (adjustment > 0.5m) { adjustment = 0.5m; }

                landingDistance = landingDistance * (1 - adjustment);

            }

            return landingDistance;
        }

    }
}