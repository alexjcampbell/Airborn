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
            }
            else if (result.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                // don't allow more than 50% adjustment
                if (adjustment > 0.5m) { adjustment = 0.5m; }

                // make the adjustment
                adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - adjustment);

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
                logItem.Add("Subtracting 10% for each 13 knots of headwind");
                adjustedLandingDistance = adjustedLandingDistance * (1 - ((result.HeadwindComponent / 13) * 0.1m));
            }
            else if (result.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots

                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                // don't allow more than 50% adjustment
                if (adjustment > 0.5m) { adjustment = 0.5m; }

                // make the adjustment
                adjustedLandingDistance = adjustedLandingDistance * (1 - adjustment);

            }

            return Distance.FromFeet(adjustedLandingDistance);
        }

    }
}