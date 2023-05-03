using System;

namespace Airborn.web.Models
{
    public class C172_SP : Aircraft
    {

        public C172_SP()
        {
        }

        public override Distance MakeTakeoffAdjustments(
            CalculationResultForRunway result,
            Distance unadjustedTakeoffDistance,
            PerformanceCalculationLogItem logItem
            )
        {
            decimal adjustedTakeoffDistance = unadjustedTakeoffDistance.TotalFeet;

            if (result.HeadwindComponent > 0)
            {

                // subtract 10% for each 9 knots of headwind
                adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - ((result.HeadwindComponent / 9) * 0.1m));

                logItem.Add("Subtracting 10% for each 9 knots of headwind");
                logItem.Add("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00"));
                logItem.Add("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00"));
                logItem.Add("Adjustment factor (headwind component / 9 * 10%): " + ((result.HeadwindComponent / 9) * 0.1m).ToString("#,##0.00"));
                logItem.Add("Adjusted takeoff distance: " + adjustedTakeoffDistance.ToString("#,##0.00"));
            }
            else if (result.HeadwindComponent < 0) // negative headwind is a tailwind
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                // make the adjustment
                adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - adjustment);

                logItem.Add("Adding 10% for each 2 knots of tailwind up to 10 knots");
                logItem.Add("Tailwind component: " + result.HeadwindComponent.ToString("#,##0.00"));
                logItem.Add("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00"));
                logItem.Add("Adjustment factor (tailwind component / 2 * 10%): " + adjustment.ToString("#,##0.00"));
                logItem.Add("Adjusted takeoff distance: " + adjustedTakeoffDistance.ToString("#,##0.00"));

            }

            return Distance.FromFeet(adjustedTakeoffDistance);
        }

        public override Distance MakeLandingAdjustments(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLogItem logItem
            )
        {
            decimal adjustedLandingDistance = unadjustedLandingDistance.TotalFeet;

            if (result.HeadwindComponent > 0)
            {
                // subtract 10% for each 9 knots of headwind
                adjustedLandingDistance = adjustedLandingDistance * (1 - ((result.HeadwindComponent / 9) * 0.1m));

                logItem.Add("Subtracting 10% for each 9 knots of headwind");
                logItem.Add("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00"));
                logItem.Add("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00"));
                logItem.Add("Adjustment factor (headwind component / 9 * 10%): " + ((result.HeadwindComponent / 9) * 0.1m).ToString("#,##0.00"));
                logItem.Add("Adjusted landing distance: " + adjustedLandingDistance.ToString("#,##0.00"));
            }
            else if (result.HeadwindComponent < 0)
            {
                // add 10% for each 2 knots of tailwind up to 10 knots
                decimal adjustment = (result.HeadwindComponent / 2) * 0.1m;

                // make the adjustment
                adjustedLandingDistance = adjustedLandingDistance * (1 - adjustment);

                logItem.Add("Adding 10% for each 2 knots of tailwind up to 10 knots");
                logItem.Add("Tailwind component: " + result.HeadwindComponent.ToString("#,##0.00"));
                logItem.Add("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00"));
                logItem.Add("Adjustment factor (tailwind component / 2 * 10%): " + adjustment.ToString("#,##0.00"));
                logItem.Add("Adjusted landing distance: " + adjustedLandingDistance.ToString("#,##0.00"));


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

        public override string GetAircraftTypeString()
        {
            return "Cessna 172SP";
        }
    }
}