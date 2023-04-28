using System;

namespace Airborn.web.Models
{
    public abstract class SR22 : Aircraft
    {

        public SR22()
        {
        }


        public override Distance MakeTakeoffAdjustments(
            PerformanceCalculationResultForRunway result,
            Distance unadjustedTakeoffDistance,
            PerformanceCalculationLogItem logItem)
        {

            decimal adjustedTakeoffDistance = unadjustedTakeoffDistance.TotalFeet;

            if (result.HeadwindComponent > 0)
            {

                // subtract 10% for each 12 knots of headwind
                adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - ((result.HeadwindComponent / 12) * 0.1m));

                logItem.Add("Subtracting 10% for each 12 knots of headwind");
                logItem.Add("Headwind component: " + result.HeadwindComponent);
                logItem.Add("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet);
                logItem.Add("Adjustment factor (headwind component / 12 * 10%): " + (result.HeadwindComponent / 12) * 0.1m);
                logItem.Add("Adjusted takeoff distance: " + adjustedTakeoffDistance);
            }
            else if (result.HeadwindComponent < 0) // negative headwinds are tailwinds
            {

                // add 10% for each 2 knots of tailwind up to 10 knots
                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                // don't allow more than 50% adjustment
                if (adjustment > 0.5m)
                {
                    logItem.Add("Adjustment capped at 50%");
                    adjustment = 0.5m;

                }

                // make the adjustment
                adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - adjustment);

                logItem.Add("Adding 10% for each 2 knots of tailwind up to 10 knots");
                logItem.Add("Tailwind component: " + result.HeadwindComponent);
                logItem.Add("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet);
                logItem.Add("Adjustment factor (tailwind component / 2 * 10%): " + (result.HeadwindComponent / 2) * 0.1m);
                logItem.Add("Adjusted takeoff distance: " + adjustedTakeoffDistance);
            }

            return Distance.FromFeet(adjustedTakeoffDistance);
        }

        public override Distance MakeLandingAdjustments(
            PerformanceCalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLogItem logItem)
        {
            decimal adjustedLandingDistance = unadjustedLandingDistance.TotalFeet;

            if (result.HeadwindComponent > 0)
            {
                // subtract 10% for each 13 knots of headwind
                adjustedLandingDistance = adjustedLandingDistance * (1 - ((result.HeadwindComponent / 13) * 0.1m));

                logItem.Add("Subtracting 10% for each 13 knots of headwind");
                logItem.Add("Headwind component: " + result.HeadwindComponent);
                logItem.Add("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet);
                logItem.Add("Adjustment factor (headwind component / 13 * 10%): " + (result.HeadwindComponent / 13) * 0.1m);
                logItem.Add("Adjusted landing distance: " + adjustedLandingDistance);
            }
            else if (result.HeadwindComponent < 0)
            {
                // negative headwinds are tailwinds

                // add 10% for each 2 knots of tailwind up to 10 knots
                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                // don't allow more than 50% adjustment
                if (adjustment > 0.5m)
                {
                    adjustment = 0.5m;
                    logItem.Add("Adjustment capped at 50%: " + adjustment);
                }

                // make the adjustment
                adjustedLandingDistance = adjustedLandingDistance * (1 - adjustment);

                logItem.Add("Adding 10% for each 2 knots of tailwind up to 10 knots");
                logItem.Add("Headwind component: " + result.HeadwindComponent);
                logItem.Add("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet);
                logItem.Add("Adjustment factor (headwind component / 2 * 10%): " + adjustment);
                logItem.Add("Adjusted landing distance: " + adjustedLandingDistance);

            }

            return Distance.FromFeet(adjustedLandingDistance);
        }

    }
}