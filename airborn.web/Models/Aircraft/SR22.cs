using System;

namespace Airborn.web.Models
{
    public abstract class SR22 : Aircraft
    {

        public SR22()
        {
        }


        public override Distance MakeTakeoffAdjustments(
            CalculationResultForRunway result,
            Distance unadjustedTakeoffDistance,
            PerformanceCalculationLogItem logItem)
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
                logItem.Add("Adding 15% for grass runway");
            }

            logItem.Add("Adjusted takeoff distance: " + adjustedTakeoffDistance.ToString("#,##0.00") + " ft");

            return Distance.FromFeet(adjustedTakeoffDistance);
        }

        private static double CalculateAdjustedTakeoffDistanceForHeadwind(CalculationResultForRunway result, Distance unadjustedTakeoffDistance, PerformanceCalculationLogItem logItem, double adjustedTakeoffDistance)
        {
            // subtract 10% for each 12 knots of headwind
            adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - ((result.HeadwindComponent / 12) * 0.1f));

            logItem.Add("Subtracting 10% for each 12 knots of headwind");
            logItem.Add("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.Add("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.Add("Adjustment factor (headwind component / 12 * 10%): " + ((result.HeadwindComponent / 12) * 0.1f).ToString("#,##0.00"));
            return adjustedTakeoffDistance;
        }

        private static double CalculateAdjustedTakeoffDistanceForTailwind(
            CalculationResultForRunway result,
            Distance unadjustedTakeoffDistance,
            PerformanceCalculationLogItem logItem,
            double adjustedTakeoffDistance
            )
        {
            // add 10% for each 2 knots of tailwind up to 10 knots
            double adjustment = (result.HeadwindComponent / 2) * 0.1f;

            // don't allow more than 50% adjustment
            if (adjustment > 0.5f)
            {
                logItem.Add("Adjustment capped at 50%");
                adjustment = 0.5f;

            }

            // make the adjustment
            adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - adjustment);

            logItem.Add("Adding 10% for each 2 knots of tailwind up to 10 knots");
            logItem.Add("Tailwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.Add("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.Add("Adjustment factor (tailwind component / 2 * 10%): " + ((result.HeadwindComponent / 2) * 0.1f).ToString("#,##0.00"));
            return adjustedTakeoffDistance;
        }

        private Distance CalculateAdjustedTakeoffDistanceForForSlope(
            double groundRollDistance,
            double slope,
            double pressureAltitude,
            PerformanceCalculationLogItem logItem
            )
        {

            logItem.Add("Runway slope: " + slope.ToString("0.00") + " %");
            logItem.Add("Runway slope adjustment factor: " + (slope * 0.1f).ToString("0.00"));

            // the POH says we should decrease landing distance for downslopes, but the slope information
            // we have is only an average, and some runways are weird and convex so instead we just don't
            // decrease landing distance for downslopes

            double slopeAdjustmentFactor = 0;

            if (pressureAltitude <= 0 && slope > 0)
            {
                slopeAdjustmentFactor = 0.22; // only provide the worst case adjustment, for uphill takeoffs
            }
            else if (pressureAltitude <= 5000 && slope > 0)
            {
                slopeAdjustmentFactor = 0.3;
            }
            else if (slope > 0)
            {
                slopeAdjustmentFactor = 0.43;
            }

            double adjustment = Math.Abs(slope) * slopeAdjustmentFactor * groundRollDistance;
            double adjustedGroundRollDistance = groundRollDistance + adjustment;

            return Distance.FromFeet(adjustedGroundRollDistance);
        }

        public override Distance MakeLandingAdjustments(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLogItem logItem)
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
                logItem.Add("Adding 20% for dry grass runway");
            }

            logItem.Add("Adjusted landing distance: " + adjustedLandingDistance.ToString("#,##0.00") + " ft");

            return Distance.FromFeet(adjustedLandingDistance);
        }

        private static double CalculateAdjustedLandingDistanceForHeadwind(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLogItem logItem,
            double adjustedLandingDistance
            )
        {
            // subtract 10% for each 13 knots of headwind
            adjustedLandingDistance = adjustedLandingDistance * (1 - ((result.HeadwindComponent / 13) * 0.1f));

            logItem.Add("Subtracting 10% for each 13 knots of headwind");
            logItem.Add("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.Add("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.Add("Adjustment factor (headwind component / 13 * 10%): " + ((result.HeadwindComponent / 13) * 0.1f).ToString("#,##0.00"));
            return adjustedLandingDistance;
        }

        private static double CalculateAdjustedLandingDistanceForTailwind(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLogItem logItem,
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
                logItem.Add("Adjustment capped at 50%: " + adjustment.ToString("#,##0.00"));
            }

            // make the adjustment
            adjustedLandingDistance = adjustedLandingDistance * (1 - adjustment);

            logItem.Add("Adding 10% for each 2 knots of tailwind up to 10 knots");
            logItem.Add("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
            logItem.Add("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00") + " ft");
            logItem.Add("Adjustment factor (headwind component / 2 * 10%): " + adjustment.ToString("#,##0.00"));
            return adjustedLandingDistance;
        }

        private static double CalculateAdjustedLandingDistanceForSlope(
            CalculationResultForRunway result,
            PerformanceCalculationLogItem logItem,
            double adjustedLandingDistance
            )
        {
            logItem.Add("Runway slope: " + result.Runway.Slope.Value.ToString("0.00") + " %");
            logItem.Add("Runway slope adjustment factor: " + (result.Runway.Slope.Value * 0.1f).ToString("0.00"));

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
            PerformanceCalculationLogItem logItem)
        {
            // tetermine the adjustment factor based on the given altitude and whether the slope is positive (upslope) or negative (downslope)
            double adjustmentFactor;

            // the POH says we should decrease landing distance for downslopes, but the slope information
            // we have is only an average, and some runways are weird and convex so instead we just don't
            // decrease landing distance for downslopes

            if (altitude > 0)
            {
                adjustmentFactor = slope > 0 ? 0.22 : 0;  // don't decrease landing distance for downslope runways

                logItem.Add("Slope adjustment factor (altitude >=0, <5,000): " + adjustmentFactor.ToString("0.00") + " for slope " + slope.ToString("0.00"));
            }
            else if (altitude <= 5000)
            {
                adjustmentFactor = slope > 0 ? 0.3 : 0;  // don't decrease landing distance for downslope runways

                logItem.Add("Slope adjustment factor (altitude >= 5,000, < 10,000): " + adjustmentFactor.ToString("0.00") + " for slope " + slope.ToString("0.00"));

            }
            else
            {
                adjustmentFactor = slope > 0 ? 0.43 : 0; // don't decrease landing distance for downslope runways

                logItem.Add("Slope adjustment factor (altitude >= 10,000): " + adjustmentFactor.ToString("0.00") + " for slope " + slope.ToString("0.00"));
            }

            // Calculate the adjustment based on the slope, adjustment factor, and ground roll distance
            double adjustment = Math.Abs(slope) * adjustmentFactor * groundRollDistance;

            // Add the adjustment to the original ground roll distance to get the adjusted ground roll distance
            double adjustedGroundRollDistance = groundRollDistance + adjustment;

            // Return the adjusted ground roll distance
            return Distance.FromFeet(adjustedGroundRollDistance);
        }

    }
}