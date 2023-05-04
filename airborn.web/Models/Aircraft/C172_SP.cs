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
            PerformanceCalculationLogItem logItem,
            AirconOptions airconOption
            )
        {
            double adjustedTakeoffDistance = unadjustedTakeoffDistance.TotalFeet;

            if (result.HeadwindComponent > 0)
            {
                adjustedTakeoffDistance = CalculateAdjustedTakeoffDistanceForHeadwind(result, unadjustedTakeoffDistance, logItem, adjustedTakeoffDistance);
            }
            else if (result.HeadwindComponent < 0) // negative headwind is a tailwind
            {
                adjustedTakeoffDistance = CalculateAdjustedTakeoffDistanceForTailwind(result, unadjustedTakeoffDistance, logItem, adjustedTakeoffDistance);
            }

            if (result.Runway.Surface == RunwaySurface.Grass)
            {
                adjustedTakeoffDistance = adjustedTakeoffDistance * 1.15f;

                logItem.Add("Adding 15% for grass runway");
            }

            return Distance.FromFeet(adjustedTakeoffDistance);
        }

        private double CalculateAdjustedTakeoffDistanceForHeadwind(
            CalculationResultForRunway result,
            Distance unadjustedTakeoffDistance,
            PerformanceCalculationLogItem logItem,
            double adjustedTakeoffDistance
            )
        {
            // subtract 10% for each 9 knots of headwind
            adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - ((result.HeadwindComponent / 9) * 0.1f));

            logItem.Add("Subtracting 10% for each 9 knots of headwind");
            logItem.Add("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " knots");
            logItem.Add("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.Add("Adjustment factor (headwind component / 9 * 10%): " + ((result.HeadwindComponent / 9) * 0.1f).ToString("#,##0.00"));
            logItem.Add("Adjusted takeoff distance: " + adjustedTakeoffDistance.ToString("#,##0.00") + " ft");
            return adjustedTakeoffDistance;
        }


        private double CalculateAdjustedTakeoffDistanceForTailwind(CalculationResultForRunway result, Distance unadjustedTakeoffDistance, PerformanceCalculationLogItem logItem, double adjustedTakeoffDistance)
        {
            // add 10% for each 2 knots of tailwind up to 10 knots
            double adjustment = (result.HeadwindComponent / 2) * 0.1f;

            // make the adjustment
            adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - adjustment);

            logItem.Add("Adding 10% for each 2 knots of tailwind up to 10 knots");
            logItem.Add("Tailwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " knots");
            logItem.Add("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.Add("Adjustment factor (tailwind component / 2 * 10%): " + adjustment.ToString("#,##0.00"));
            logItem.Add("Adjusted takeoff distance: " + adjustedTakeoffDistance.ToString("#,##0.00") + " ft");
            return adjustedTakeoffDistance;
        }


        public override Distance MakeLandingAdjustments(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLogItem logItem,
            AirconOptions airconOption
            )
        {
            double adjustedLandingDistance = unadjustedLandingDistance.TotalFeet;

            if (result.HeadwindComponent > 0)
            {
                adjustedLandingDistance = CalculateAdjustedLandingDistanceForHeadwind(result, unadjustedLandingDistance, logItem, adjustedLandingDistance);
            }
            else if (result.HeadwindComponent < 0)
            {
                adjustedLandingDistance = CalculateAdjustedLandingDistanceForTailwind(result, unadjustedLandingDistance, logItem, adjustedLandingDistance);

            }

            if (result.Runway.Surface == RunwaySurface.Grass)
            {
                adjustedLandingDistance = adjustedLandingDistance * 1.45f;

                logItem.Add("Adding 45% for grass runway");
            }

            return Distance.FromFeet(adjustedLandingDistance);
        }


        private double CalculateAdjustedLandingDistanceForHeadwind(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLogItem logItem,
            double adjustedLandingDistance
            )
        {
            // subtract 10% for each 9 knots of headwind
            adjustedLandingDistance = adjustedLandingDistance * (1 - ((result.HeadwindComponent / 9) * 0.1f));

            logItem.Add("Subtracting 10% for each 9 knots of headwind");
            logItem.Add("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.Add("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.Add("Adjustment factor (headwind component / 9 * 10%): " + ((result.HeadwindComponent / 9) * 0.1f).ToString("#,##0.00"));
            logItem.Add("Adjusted landing distance: " + adjustedLandingDistance.ToString("#,##0.00"));
            return adjustedLandingDistance;
        }

        private double CalculateAdjustedLandingDistanceForTailwind(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLogItem logItem,
            double adjustedLandingDistance
            )
        {
            // add 10% for each 2 knots of tailwind up to 10 knots
            double adjustment = (result.HeadwindComponent / 2) * 0.1f;

            // make the adjustment
            adjustedLandingDistance = adjustedLandingDistance * (1 - adjustment);

            logItem.Add("Adding 10% for each 2 knots of tailwind up to 10 knots");
            logItem.Add("Tailwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.Add("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.Add("Adjustment factor (tailwind component / 2 * 10%): " + adjustment.ToString("#,##0.00"));
            logItem.Add("Adjusted landing distance: " + adjustedLandingDistance.ToString("#,##0.00") + " ft");
            return adjustedLandingDistance;
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

        public override bool HasAirconOption()
        {
            return false;
        }

        public override string GetAircraftTypeString()
        {
            return "Cessna 172SP";
        }
    }
}