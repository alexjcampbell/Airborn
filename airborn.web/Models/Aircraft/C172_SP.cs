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
            PerformanceCalculationLog.LogItem logItem,
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

                logItem.AddSubItem("Adding 15% for grass runway");
            }

            return Distance.FromFeet(adjustedTakeoffDistance);
        }

        private double CalculateAdjustedTakeoffDistanceForHeadwind(
            CalculationResultForRunway result,
            Distance unadjustedTakeoffDistance,
            PerformanceCalculationLog.LogItem logItem,
            double adjustedTakeoffDistance
            )
        {
            // subtract 10% for each 9 knots of headwind
            adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - ((result.HeadwindComponent / 9) * 0.1f));

            logItem.AddSubItem("Subtracting 10% for each 9 knots of headwind");
            logItem.AddSubItem("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " knots");
            logItem.AddSubItem("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.AddSubItem("Adjustment factor (headwind component / 9 * 10%): " + ((result.HeadwindComponent / 9) * 0.1f).ToString("#,##0.00"));
            logItem.AddSubItem("Adjusted takeoff distance: " + adjustedTakeoffDistance.ToString("#,##0.00") + " ft");
            return adjustedTakeoffDistance;
        }


        private double CalculateAdjustedTakeoffDistanceForTailwind(
            CalculationResultForRunway result,
            Distance unadjustedTakeoffDistance,
            PerformanceCalculationLog.LogItem logItem,
            double adjustedTakeoffDistance)
        {
            // add 10% for each 2 knots of tailwind up to 10 knots
            double adjustment = (result.HeadwindComponent / 2) * 0.1f;

            // make the adjustment
            adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - adjustment);

            logItem.AddSubItem("Adding 10% for each 2 knots of tailwind up to 10 knots");
            logItem.AddSubItem("Tailwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " knots");
            logItem.AddSubItem("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.AddSubItem("Adjustment factor (tailwind component / 2 * 10%): " + adjustment.ToString("#,##0.00"));
            logItem.AddSubItem("Adjusted takeoff distance: " + adjustedTakeoffDistance.ToString("#,##0.00") + " ft");
            return adjustedTakeoffDistance;
        }


        public override Distance MakeLandingAdjustments(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLog.LogItem logItem,
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

                logItem.AddSubItem("Adding 45% for grass runway");
            }

            return Distance.FromFeet(adjustedLandingDistance);
        }


        private double CalculateAdjustedLandingDistanceForHeadwind(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLog.LogItem logItem,
            double adjustedLandingDistance
            )
        {
            // subtract 10% for each 9 knots of headwind
            adjustedLandingDistance = adjustedLandingDistance * (1 - ((result.HeadwindComponent / 9) * 0.1f));

            logItem.AddSubItem("Subtracting 10% for each 9 knots of headwind");
            logItem.AddSubItem("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.AddSubItem("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.AddSubItem("Adjustment factor (headwind component / 9 * 10%): " + ((result.HeadwindComponent / 9) * 0.1f).ToString("#,##0.00"));
            logItem.AddSubItem("Adjusted landing distance: " + adjustedLandingDistance.ToString("#,##0.00"));
            return adjustedLandingDistance;
        }

        private double CalculateAdjustedLandingDistanceForTailwind(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLog.LogItem logItem,
            double adjustedLandingDistance
            )
        {
            // add 10% for each 2 knots of tailwind up to 10 knots
            double adjustment = (result.HeadwindComponent / 2) * 0.1f;

            // make the adjustment
            adjustedLandingDistance = adjustedLandingDistance * (1 - adjustment);

            logItem.AddSubItem("Adding 10% for each 2 knots of tailwind up to 10 knots");
            logItem.AddSubItem("Tailwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.AddSubItem("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.AddSubItem("Adjustment factor (tailwind component / 2 * 10%): " + adjustment.ToString("#,##0.00"));
            logItem.AddSubItem("Adjusted landing distance: " + adjustedLandingDistance.ToString("#,##0.00") + " ft");
            return adjustedLandingDistance;
        }

        public override string JsonFileName_LowestWeight
        {
            get
            {
                return "../Data/C172_SP_2200.json";
            }
        }


        public override string JsonFileName_HighestWeight
        {
            get
            {
                return "../Data/C172_SP_2550.json";
            }
        }

        public override int LowestPossibleWeight
        {
            get
            {
                return 2200;
            }
        }

        public override int HighestPossibleWeight
        {
            get
            {
                return 2550;
            }
        }

        public override int LowestPossibleTemperature
        {
            get
            {
                return 0;
            }
        }

        public override int HighestPossibleTemperature
        {
            get
            {
                return 40;
            }
        }

        public override int LowestPossiblePressureAltitude
        {
            get
            {
                return 0;
            }
        }

        public override int HighestPossiblePressureAltitude
        {
            get
            {
                return 8000;
            }
        }

        public override bool HasAirconOption
        {
            get
            {
                return false;
            }
        }

        public override string AircraftTypeString
        {
            get
            {
                return "Cessna 172SP";
            }
        }
    }
}