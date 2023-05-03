using System;

namespace Airborn.web.Models.PerformanceData
{
    public static class PerformanceDataInterpolationUtilities
    {

        /// <summary>
        /// This function gives you an interpolation factor between two values:
        /// If you give it a value of 5 and the lower and upper bounds are 0 and 10,
        /// it will return 0.5
        /// </summary>
        public static double CalculateInterpolationFactor(double value, double lowerBound, double upperBound)
        {
            // if the value is the same as the lower bound, there's no interpolation to be done here, so return 0
            if (value == lowerBound) { return 0; }

            if (value < 0) { throw new ArgumentOutOfRangeException(value.ToString()); }
            if (lowerBound >= upperBound) { throw new ArgumentOutOfRangeException(); }
            if (value < lowerBound) { throw new ArgumentOutOfRangeException(); }
            if (value > upperBound) { throw new ArgumentOutOfRangeException(); }

            double interpolationFactor = (value - lowerBound) / (upperBound - lowerBound);

            return interpolationFactor;
        }

        /// <summary>
        /// POH data is provided in intervals, e.g. landing distance is provided for pressure altitudes of 1000, 2000, 3000, etc.
        /// This function returns the lower bound for a given value and desired interval
        /// </summary>
        /// <param name="value">The value to interpolate</param>
        /// <param name="desiredInterval">The interval to use for interpolation</param>
        public static double GetLowerBoundForInterpolation(double value, double desiredInterval)
        {
            if (desiredInterval <= 0) { throw new ArgumentOutOfRangeException(); }
            if (value < 0) { throw new ArgumentOutOfRangeException(); }

            // if the value is 360 and the desired interval is 100, we want to return 300
            // so we need to subtract the remainder of 360 / 100 from 360
            // 360 % 100 = 60
            // 360 - 60 = 300
            return value - (value % desiredInterval);

        }

        /// <summary>
        /// POH data is provided in intervals, e.g. landing distance is provided for pressure altitudes of 1000, 2000, 3000, etc.
        /// This function returns the upper bound for a given value and desired interval
        /// </summary>
        /// <param name="value">The value to interpolate</param>
        /// <param name="desiredInterval">The interval to use for interpolation</param>
        public static double GetUpperBoundForInterpolation(double value, double desiredInterval)
        {
            if (desiredInterval <= 0) { throw new ArgumentOutOfRangeException(); }
            if (value < 0) { throw new ArgumentOutOfRangeException(); }

            double lowerBound = GetLowerBoundForInterpolation(value, desiredInterval);
            double upperBound = lowerBound + desiredInterval;

            return upperBound;
        }

        /// <summary>
        /// POH data is provided in intervals, e.g. landing distance is provided for pressure altitudes of 1000, 2000, 3000, etc.
        /// This function returns the upper and lower bounds for a given value and desired interval
        /// </summary>
        /// <param name="value">The value to interpolate</param>
        /// <param name="desiredInterval">The interval to use for interpolation</param>
        public static (double, double) GetUpperAndLowBoundsForInterpolation(double value, double desiredInterval)
        {
            if (desiredInterval <= 0) { throw new ArgumentOutOfRangeException(); }
            if (value < 0) { throw new ArgumentOutOfRangeException(); }

            double lowerBound = GetLowerBoundForInterpolation(value, desiredInterval);
            double upperBound = GetUpperBoundForInterpolation(value, desiredInterval);

            return (lowerBound, upperBound);
        }

        /// <summary>
        /// Interpolates between two values
        /// </summary>
        /// <param name="lowerValue">The lower of two values to interpolate beteeen (e.g. landing distance of 360 for a pressure altitude of 1000)</param>z
        /// <param name="upperValue">The higher of two values to interpolate beteeen (e.g. landing distance of 440 for a pressure altitude of 1000)</param>
        /// <param name="valueForInterpolation">The value to interpolate by (e.g. actual pressure altitude is 1500)</param>z
        /// <param name="desiredInterval"> The interval at which values is provided in the underlying data (e.g. for pressure altitude, always 1000)</param>
        public static double Interpolate(double lowerValue, double upperValue, double valueForInterpolation, int desiredInterval)
        {

            if (valueForInterpolation < 0)
            {
                // if the value for interpolation is less than 0, we can't interpolate
                throw new ArgumentOutOfRangeException(valueForInterpolation.ToString());
            }

            if (valueForInterpolation > valueForInterpolation + desiredInterval)
            {
                // if the value for interpolation is greater than the upper bound, we can't interpolate
                throw new ArgumentOutOfRangeException();
            }

            if (desiredInterval <= 0)
            {
                // if the desired interval is less than or equal to 0, we can't interpolate
                throw new ArgumentOutOfRangeException();
            }

            double lowerbound = GetLowerBoundForInterpolation(valueForInterpolation, desiredInterval);
            double upperBound = GetUpperBoundForInterpolation(valueForInterpolation, desiredInterval);

            double interpolationFactor = CalculateInterpolationFactor(valueForInterpolation, lowerbound, upperBound);

            double interpolatedValue =
                (upperValue - lowerValue)
                *
                interpolationFactor
                + lowerValue;

            return interpolatedValue;

        }

        /// <summary>
        /// Interpolates between two distances based on the weight interpolation factor
        /// i.e. if the weight interpolation factor is 0.5, it will return the average of the two distances
        /// </summary>
        public static double InterpolateDistanceByWeight(double weightInterpolationFactor, double distance_LowerWeight, double distance_HigherWeight)
        {
            if (weightInterpolationFactor < 0) { throw new ArgumentOutOfRangeException(weightInterpolationFactor.ToString()); }
            if (weightInterpolationFactor > 1) { throw new ArgumentOutOfRangeException(weightInterpolationFactor.ToString()); }
            if (distance_LowerWeight < 0) { throw new ArgumentOutOfRangeException(distance_LowerWeight.ToString()); }
            if (distance_HigherWeight < 0) { throw new ArgumentOutOfRangeException(distance_HigherWeight.ToString()); }

            return distance_LowerWeight +
                (weightInterpolationFactor * (distance_HigherWeight - distance_LowerWeight));
        }
    }
}