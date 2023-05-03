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

            decimal adjustedTakeoffDistance = unadjustedTakeoffDistance.TotalFeet;

            if (result.HeadwindComponent > 0)
            {

                // subtract 10% for each 12 knots of headwind
                adjustedTakeoffDistance = adjustedTakeoffDistance * (1 - ((result.HeadwindComponent / 12) * 0.1m));

                logItem.Add("Subtracting 10% for each 12 knots of headwind");
                logItem.Add("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
                logItem.Add("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00") + " ft");
                logItem.Add("Adjustment factor (headwind component / 12 * 10%): " + ((result.HeadwindComponent / 12) * 0.1m).ToString("#,##0.00"));
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
                logItem.Add("Tailwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
                logItem.Add("Unadjusted takeoff distance: " + unadjustedTakeoffDistance.TotalFeet.ToString("#,##0.00") + " ft");
                logItem.Add("Adjustment factor (tailwind component / 2 * 10%): " + ((result.HeadwindComponent / 2) * 0.1m).ToString("#,##0.00"));

            }

            if (result.Runway.Slope.HasValue && result.Runway.Slope > 0)
            {
                logItem.Add("Runway slope: " + result.Runway.Slope.Value.ToString("0.00") + " %");
                logItem.Add("Runway slope adjustment factor: " + (result.Runway.Slope.Value * 0.1m).ToString("0.00"));

                adjustedTakeoffDistance =
                    CalculateAdjustedGroundRollDistanceForTakeoff(
                        (double)adjustedTakeoffDistance,
                        (double)result.Runway.Slope.Value,
                        (double)result.PressureAltitude.TotalFeet
                        ).TotalFeet
                ;
            }

            logItem.Add("Adjusted takeoff distance: " + adjustedTakeoffDistance.ToString("#,##0.00") + " ft");

            return Distance.FromFeet(adjustedTakeoffDistance);
        }

        public Distance CalculateAdjustedGroundRollDistanceForTakeoff(double groundRollDistance, double slope, double pressureAltitude)
        {
            double adjustmentFactor;

            if (pressureAltitude <= 0)
            {
                adjustmentFactor = slope > 0 ? 0.22 : -0.07;
            }
            else if (pressureAltitude <= 5000)
            {
                adjustmentFactor = slope > 0 ? 0.3 : -0.1;
            }
            else
            {
                adjustmentFactor = slope > 0 ? 0.43 : -0.14;
            }

            double adjustment = Math.Abs(slope) * adjustmentFactor * groundRollDistance;
            double adjustedGroundRollDistance = groundRollDistance + adjustment;

            return Distance.FromFeet((decimal)adjustedGroundRollDistance);
        }

        public override Distance MakeLandingAdjustments(
            CalculationResultForRunway result,
            Distance unadjustedLandingDistance,
            PerformanceCalculationLogItem logItem)
        {
            decimal adjustedLandingDistance = unadjustedLandingDistance.TotalFeet;

            if (result.HeadwindComponent > 0)
            {
                // subtract 10% for each 13 knots of headwind
                adjustedLandingDistance = adjustedLandingDistance * (1 - ((result.HeadwindComponent / 13) * 0.1m));

                logItem.Add("Subtracting 10% for each 13 knots of headwind");
                logItem.Add("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
                logItem.Add("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00") + " ft");
                logItem.Add("Adjustment factor (headwind component / 13 * 10%): " + ((result.HeadwindComponent / 13) * 0.1m).ToString("#,##0.00"));
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
                    logItem.Add("Adjustment capped at 50%: " + adjustment.ToString("#,##0.00"));
                }

                // make the adjustment
                adjustedLandingDistance = adjustedLandingDistance * (1 - adjustment);

                logItem.Add("Adding 10% for each 2 knots of tailwind up to 10 knots");
                logItem.Add("Headwind component: " + result.HeadwindComponent.ToString("#,##0.00") + " kts");
                logItem.Add("Unadjusted landing distance: " + unadjustedLandingDistance.TotalFeet.ToString("#,##0.00") + " ft");
                logItem.Add("Adjustment factor (headwind component / 2 * 10%): " + adjustment.ToString("#,##0.00"));

            }

            if (result.Runway.Slope.HasValue && result.Runway.Slope > 0)
            {
                logItem.Add("Runway slope: " + result.Runway.Slope.Value.ToString("0.00") + " %");
                logItem.Add("Runway slope adjustment factor: " + (result.Runway.Slope.Value * 0.1m).ToString("0.00"));

                adjustedLandingDistance =
                    CalculateAdjustedGroundRollDistanceForLanding(
                        (double)adjustedLandingDistance,
                        (double)result.Runway.Slope.Value,
                        (double)result.PressureAltitude.TotalFeet,
                        logItem
                        ).TotalFeet
                ;
            }

            logItem.Add("Adjusted landing distance: " + adjustedLandingDistance.ToString("#,##0.00") + " ft");

            return Distance.FromFeet(adjustedLandingDistance);
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
            // Determine the adjustment factor based on the given altitude and whether the slope is positive (upslope) or negative (downslope)
            double adjustmentFactor;

            if (altitude > 0)
            {
                adjustmentFactor = slope > 0 ? 0.22 : -0.07;

                logItem.Add("Slope adjustment factor (altitude >=0, <5,000): " + adjustmentFactor.ToString("0.00") + " for slope " + slope.ToString("0.00"));
            }
            else if (altitude <= 5000)
            {
                adjustmentFactor = slope > 0 ? 0.3 : -0.1;

                logItem.Add("Slope adjustment factor (altitude >= 5,000, < 10,000): " + adjustmentFactor.ToString("0.00") + " for slope " + slope.ToString("0.00"));

            }
            else
            {
                adjustmentFactor = slope > 0 ? 0.43 : -0.14;

                logItem.Add("Slope adjustment factor (altitude >= 10,000): " + adjustmentFactor.ToString("0.00") + " for slope " + slope.ToString("0.00"));
            }

            // Calculate the adjustment based on the slope, adjustment factor, and ground roll distance
            double adjustment = Math.Abs(slope) * adjustmentFactor * groundRollDistance;

            // Add the adjustment to the original ground roll distance to get the adjusted ground roll distance
            double adjustedGroundRollDistance = groundRollDistance + adjustment;

            // Return the adjusted ground roll distance
            return Distance.FromFeet((decimal)adjustedGroundRollDistance);
        }

    }
}