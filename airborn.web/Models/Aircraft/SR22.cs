using System;

namespace Airborn.web.Models
{
    public abstract class SR22 : Aircraft
    {

        public SR22(Scenario scenario, string path) : base(scenario)
        {
        }

        public override double MakeTakeoffAdjustments(double takeoffDistance)
        {

            if (Scenario.HeadwindComponent > 0)
            {
                // subtract 10% for each 12 knots of headwind
                takeoffDistance = takeoffDistance * (1 - ((Scenario.HeadwindComponent / 12) * 0.1));
            }
            else if (Scenario.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                double adjustment = (Scenario.HeadwindComponent / 2) * 0.1;

                if (adjustment > 0.5) { adjustment = 0.5; }

                takeoffDistance = takeoffDistance * (1 - adjustment);

            }



            return takeoffDistance;
        }

        public override double MakeLandingAdjustments(double landingDistance)
        {
            if (Scenario.HeadwindComponent > 0)
            {
                // subtract 10% for each 13 knots of headwind
                landingDistance = landingDistance * (1 - ((Scenario.HeadwindComponent / 13) * 0.1));
            }
            else if (Scenario.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                double adjustment = (Scenario.HeadwindComponent / 2) * 0.1;

                if (adjustment > 0.5) { adjustment = 0.5; }

                landingDistance = landingDistance * (1 - adjustment);

            }

            return landingDistance;
        }

    }
}