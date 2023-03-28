using System;

namespace Airborn.web.Models
{
    public abstract class SR22 : Aircraft
    {

        public SR22()
        {
        }


        public override double MakeTakeoffAdjustments(PerformanceCalculationResult result, double takeoffDistance)
        {

            if (result.HeadwindComponent > 0)
            {
                // subtract 10% for each 12 knots of headwind
                takeoffDistance = takeoffDistance * (1 - ((result.HeadwindComponent / 12) * 0.1));
            }
            else if (result.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                double adjustment = (result.HeadwindComponent / 2) * 0.1;

                if (adjustment > 0.5) { adjustment = 0.5; }

                takeoffDistance = takeoffDistance * (1 - adjustment);

            }



            return takeoffDistance;
        }

        public override double MakeLandingAdjustments(PerformanceCalculationResult result, double landingDistance)
        {
            if (result.HeadwindComponent > 0)
            {
                // subtract 10% for each 13 knots of headwind
                landingDistance = landingDistance * (1 - ((result.HeadwindComponent / 13) * 0.1));
            }
            else if (result.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                double adjustment = (result.HeadwindComponent / 2) * 0.1;

                if (adjustment > 0.5) { adjustment = 0.5; }

                landingDistance = landingDistance * (1 - adjustment);

            }

            return landingDistance;
        }

    }
}