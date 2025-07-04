using System;

namespace Airborn.web.Models
{
    public abstract class SR22 : Aircraft
    {

        public SR22()
        {
        }

        public override int LowestPossiblePressureAltitude
        {
            get { return 0; }
        }

        public override int HighestPossiblePressureAltitude
        {
            get { return 10000; }
        }



        public override Distance MakeTakeoffAdjustments(
            CalculationResultForRunway result,
            Distance unadjustedTakeoffDistance,
            PerformanceCalculationLog.LogItem logItem,
            AirconOptions airconOption
            )
        {

            double adjustedTakeoffDistance = unadjustedTakeoffDistance.TotalFeet;

            // adjust for headwind
            if (result.HeadwindComponent > 0)
            {
                adjustedTakeoffDistance = CalculateAdjustedTakeoffDistanceForHeadwind(result, unadjustedTakeoffDistance, logItem, adjustedTakeoffDistance);
            }
            else if (result.HeadwindComponent < 0) // negative headwinds are tailwinds
            {
                adjustedTakeoffDistance = CalculateAdjustedTakeoffDistanceForTailwind(result, unadjustedTakeoffDistance, logItem, adjustedTakeoffDistance);

            }

            if (result.Runway.Slope.HasValue && result.Runway.Slope > 0)
            {
                adjustedTakeoffDistance =
                    CalculateAdjustedTakeoffDistanceForForSlope(
                        adjustedTakeoffDistance,
                        result.Runway.Slope.Value,
                        result.PressureAltitude.TotalFeet,
                        logItem
                        ).TotalFeet
                ;
            }

            if (result.Runway.Surface == RunwaySurface.Grass)
            {
                adjustedTakeoffDistance = adjustedTakeoffDistance * 1.15f;
                logItem.AddSubItem("Adding 15% for grass runway");
            }

            if (airconOption == AirconOptions.Aircon)
            {
                // todo: this should be ground roll +100, 50' distance +150
                adjustedTakeoffDistance = adjustedTakeoffDistance + 150;
                logItem.AddSubItem("Adding 150 ft to takeoff roll for air conditioning on");
            }

            logItem.AddSubItem("Adjusted takeoff distance: " + adjustedTakeoffDistance.ToString("#,##0.00") + " ft");

            return Distance.FromFeet(adjustedTakeoffDistance);
        }

        private double CalculateAdjustedTakeoffDistanceForHeadwind(
            CalculationResultForRunway result, Distance unadjustedTakeoffDistance,
            PerformanceCalculationLog.LogItem logItem,
            double adjustedTakeoffDistance)
        {
            // subtract 10% for each 12 knots of headwind
            adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - ((result.HeadwindComponent / 12) * 0.1f));

            logItem.AddSubItem("Subtracting 10% for each 12 knots of headwind");
            logItem.AddSubItem("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.AddSubItem("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.AddSubItem("Adjustment factor (headwind component / 12 * 10%): " + ((result.HeadwindComponent / 12) * 0.1f).ToString("#,##0.00"));
            return adjustedTakeoffDistance;
        }

        private double CalculateAdjustedTakeoffDistanceForTailwind(
            CalculationResultForRunway result,
            Distance unadjustedTakeoffDistance,
            PerformanceCalculationLog.LogItem logItem,
            double adjustedTakeoffDistance
            )
        {
            // add 10% for each 2 knots of tailwind up to 10 knots
            double adjustment = (result.HeadwindComponent / 2) * 0.1f;

            // don't allow more than 50% adjustment
            if (adjustment > 0.5f)
            {
                logItem.AddSubItem("Adjustment capped at 50%");
                adjustment = 0.5f;

            }

            // make the adjustment
            adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - adjustment);

            logItem.AddSubItem("Adding 10% for each 2 knots of tailwind up to 10 knots");
            logItem.AddSubItem("Tailwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.AddSubItem("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.AddSubItem("Adjustment factor (tailwind component / 2 * 10%): " + ((result.HeadwindComponent / 2) * 0.1f).ToString("#,##0.00"));
            return adjustedTakeoffDistance;
        }

        private Distance CalculateAdjustedTakeoffDistanceForForSlope(
            double groundRollDistance,
            double slope,
            double pressureAltitude,
            PerformanceCalculationLog.LogItem logItem
            )
        {
            // Per POH: Sloped Runways - Increase distances by 22% of the ground roll value at Sea
            // Level, 30% of the ground roll value at 5000 ft, 43% of the ground roll value at
            // 10000 ft for each 1% of upslope; decrease distances by 7% of the ground roll
            // value at Sea Level, 10% of the ground roll value at 5000 ft, and 14% of the
            // ground roll value at 10000 ft for each 1% of downslope.

            logItem.AddSubItem("Runway slope: " + slope.ToString("0.00") + " %");

            // the POH says we should decrease takeoff distance for downslopes, but the slope information
            // we have is only an average, and some runways are weird and convex so instead we just don't
            // decrease takeoff distance for downslopes

            double slopeAdjustmentFactor = 0;

            // Interpolate adjustment factors based on altitude
            if (pressureAltitude <= 0)
            {
                // At or below sea level, use sea level values
                slopeAdjustmentFactor = slope > 0 ? 0.22 : 0; // only provide the worst case adjustment, for uphill takeoffs
                logItem.AddSubItem($"Slope adjustment factor (sea level): {slopeAdjustmentFactor:0.00}");
            }
            else if (pressureAltitude <= 5000)
            {
                // Interpolate between sea level (0 ft) and 5000 ft
                double seaLevelFactor = slope > 0 ? 0.22 : 0;
                double fiveThousandFactor = slope > 0 ? 0.30 : 0;
                slopeAdjustmentFactor = seaLevelFactor + (fiveThousandFactor - seaLevelFactor) * (pressureAltitude / 5000.0);
                logItem.AddSubItem($"Slope adjustment factor (interpolated 0-5000 ft): {slopeAdjustmentFactor:0.00}");
            }
            else if (pressureAltitude <= 10000)
            {
                // Interpolate between 5000 ft and 10000 ft
                double fiveThousandFactor = slope > 0 ? 0.30 : 0;
                double tenThousandFactor = slope > 0 ? 0.43 : 0;
                slopeAdjustmentFactor = fiveThousandFactor + (tenThousandFactor - fiveThousandFactor) * ((pressureAltitude - 5000.0) / 5000.0);
                logItem.AddSubItem($"Slope adjustment factor (interpolated 5000-10000 ft): {slopeAdjustmentFactor:0.00}");
            }
            else
            {
                // Above 10000 ft, use 10000 ft values
                slopeAdjustmentFactor = slope > 0 ? 0.43 : 0;
                logItem.AddSubItem($"Slope adjustment factor (above 10000 ft): {slopeAdjustmentFactor:0.00}");
            }

            double adjustment = Math.Abs(slope) * slopeAdjustmentFactor * groundRollDistance;
            double adjustedGroundRollDistance = groundRollDistance + adjustment;

            logItem.AddSubItem($"Slope adjustment: {adjustment:0.00} ft");

            return Distance.FromFeet(adjustedGroundRollDistance);
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

            if (result.Runway.Slope.HasValue && result.Runway.Slope > 0)
            {
                adjustedLandingDistance = CalculateAdjustedLandingDistanceForSlope(result, logItem, adjustedLandingDistance);
            }

            if (result.Runway.Surface == RunwaySurface.Grass)
            {
                adjustedLandingDistance = adjustedLandingDistance * 1.20f;
                logItem.AddSubItem("Adding 20% for dry grass runway");
            }

            logItem.AddSubItem("Adjusted landing distance: " + adjustedLandingDistance.ToString("#,##0.00") + " ft");

            return Distance.FromFeet(adjustedLandingDistance);
        }

        private static double CalculateAdjustedLandingDistanceForHeadwind(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLog.LogItem logItem,
            double adjustedLandingDistance
            )
        {
            // subtract 10% for each 13 knots of headwind
            adjustedLandingDistance = adjustedLandingDistance * (1 - ((result.HeadwindComponent / 13) * 0.1f));

            logItem.AddSubItem("Subtracting 10% for each 13 knots of headwind");
            logItem.AddSubItem("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.AddSubItem("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.AddSubItem("Adjustment factor (headwind component / 13 * 10%): " + ((result.HeadwindComponent / 13) * 0.1f).ToString("#,##0.00"));
            return adjustedLandingDistance;
        }

        private static double CalculateAdjustedLandingDistanceForTailwind(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLog.LogItem logItem,
            double adjustedLandingDistance
            )
        {
            // negative headwinds are tailwinds

            // add 10% for each 2 knots of tailwind up to 10 knots
            double adjustment = (result.HeadwindComponent / 2) * 0.1f;

            // don't allow more than 50% adjustment
            if (adjustment > 0.5f)
            {
                adjustment = 0.5f;
                logItem.AddSubItem("Adjustment capped at 50%: " + adjustment.ToString("#,##0.00"));
            }

            // make the adjustment
            adjustedLandingDistance = adjustedLandingDistance * (1 - adjustment);

            logItem.AddSubItem("Adding 10% for each 2 knots of tailwind up to 10 knots");
            logItem.AddSubItem("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.AddSubItem("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.AddSubItem("Adjustment factor (headwind component / 2 * 10%): " + adjustment.ToString("#,##0.00"));
            return adjustedLandingDistance;
        }

        private static double CalculateAdjustedLandingDistanceForSlope(
            CalculationResultForRunway result,
            PerformanceCalculationLog.LogItem logItem,
            double adjustedLandingDistance
            )
        {
            logItem.AddSubItem("Runway slope: " + result.Runway.Slope.Value.ToString("0.00") + " %");
            logItem.AddSubItem("Runway slope adjustment factor: " + (result.Runway.Slope.Value * 0.1f).ToString("0.00"));

            adjustedLandingDistance =
                CalculateAdjustedGroundRollDistanceForLanding(
                    adjustedLandingDistance,
                    result.Runway.Slope.Value,
                    result.PressureAltitude.TotalFeet,
                    logItem
                    ).TotalFeet
            ;
            return adjustedLandingDistance;
        }


        /// <summary>
        /// Calculates the adjusted ground roll distance for a sloped runway based on the provided ground roll distance, slope, and altitude.
        /// </summary>
        /// <param name="groundRollDistance">The ground roll distance in feet.</param>
        /// <param name="slope">The runway slope as a percentage.</param>
        /// <param name="altitude">The altitude in feet.</param>
        /// <param name="logItem">The log item to which to add any log messages.</param>
        /// <returns>The adjusted ground roll distance in feet  .</returns>
        public static Distance CalculateAdjustedGroundRollDistanceForLanding(
            double groundRollDistance,
            double slope,
            double altitude,
            PerformanceCalculationLog.LogItem logItem)
        {
            // Per POH: Sloped Runways - Increase distances by 22% of the ground roll value at Sea
            // Level, 30% of the ground roll value at 5000 ft, 43% of the ground roll value at
            // 10000 ft for each 1% of upslope; decrease distances by 7% of the ground roll
            // value at Sea Level, 10% of the ground roll value at 5000 ft, and 14% of the
            // ground roll value at 10000 ft for each 1% of downslope.
            
            double adjustmentFactor;

            // the POH says we should decrease landing distance for downslopes, but the slope information
            // we have is only an average, and some runways are weird and convex so instead we just don't
            // decrease landing distance for downslopes

            // Interpolate adjustment factors based on altitude
            if (altitude <= 0)
            {
                // At or below sea level, use sea level values
                adjustmentFactor = slope > 0 ? 0.22 : 0;  // don't decrease landing distance for downslope runways
                logItem.AddSubItem($"Slope adjustment factor (sea level): {adjustmentFactor:0.00} for slope {slope:0.00}%");
            }
            else if (altitude <= 5000)
            {
                // Interpolate between sea level (0 ft) and 5000 ft
                double seaLevelFactor = slope > 0 ? 0.22 : 0;
                double fiveThousandFactor = slope > 0 ? 0.30 : 0;
                adjustmentFactor = seaLevelFactor + (fiveThousandFactor - seaLevelFactor) * (altitude / 5000.0);
                logItem.AddSubItem($"Slope adjustment factor (interpolated 0-5000 ft): {adjustmentFactor:0.00} for slope {slope:0.00}%");
            }
            else if (altitude <= 10000)
            {
                // Interpolate between 5000 ft and 10000 ft
                double fiveThousandFactor = slope > 0 ? 0.30 : 0;
                double tenThousandFactor = slope > 0 ? 0.43 : 0;
                adjustmentFactor = fiveThousandFactor + (tenThousandFactor - fiveThousandFactor) * ((altitude - 5000.0) / 5000.0);
                logItem.AddSubItem($"Slope adjustment factor (interpolated 5000-10000 ft): {adjustmentFactor:0.00} for slope {slope:0.00}%");
            }
            else
            {
                // Above 10000 ft, use 10000 ft values
                adjustmentFactor = slope > 0 ? 0.43 : 0; // don't decrease landing distance for downslope runways
                logItem.AddSubItem($"Slope adjustment factor (above 10000 ft): {adjustmentFactor:0.00} for slope {slope:0.00}%");
            }

            // Calculate the adjustment based on the slope, adjustment factor, and ground roll distance
            double adjustment = Math.Abs(slope) * adjustmentFactor * groundRollDistance;

            // Add the adjustment to the original ground roll distance to get the adjusted ground roll distance
            double adjustedGroundRollDistance = groundRollDistance + adjustment;

            // Return the adjusted ground roll distance
            return Distance.FromFeet(adjustedGroundRollDistance);
        }

        public override bool HasAirconOption
        {
            get
            {
                return true;
            }
        }

    }
}