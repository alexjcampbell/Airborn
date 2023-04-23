using System;

namespace Airborn.web.Models
{
    public static class PerformanceDataInterpolationUtilities
    {

        /// <summary>
        /// This function gives you an interpolation factor between two values:
        /// If you give it a value of 5 and the lower and upper bounds are 0 and 10,
        /// it will return 0.5
        /// </summary>
        public static decimal CalculateInterpolationFactor(decimal value, decimal lowerBound, decimal upperBound)
        {
            if (value < 0) { throw new ArgumentOutOfRangeException(value.ToString()); }
            if (lowerBound >= upperBound) { throw new ArgumentOutOfRangeException(); }
            if (value < lowerBound) { throw new ArgumentOutOfRangeException(); }
            if (value > upperBound) { throw new ArgumentOutOfRangeException(); }

            decimal interpolationFactor = (decimal)(value - lowerBound) / (decimal)(upperBound - lowerBound);

            return interpolationFactor;
        }

        /// <summary>
        /// This function gives you an interpolated value between two values:
        /// If you give it a value of 5 and the lower and upper bounds are 0 and 10,
        /// it will return 5
        /// </summary>
        /// <param name="value">The value to interpolate</param>
        /// <param name="desiredInterval">The interval to use for interpolation</param>
        public static decimal GetLowerBoundForInterpolation(decimal value, decimal desiredInterval)
        {
            if (desiredInterval <= 0) { throw new ArgumentOutOfRangeException(); }
            if (value < 0) { throw new ArgumentOutOfRangeException(); }

            // if the value is 360 and the desired interval is 100, we want to return 300
            // so we need to subtract the remainder of 360 / 100 from 360
            // 360 % 100 = 60
            // 360 - 60 = 300
            return value - (value % desiredInterval);

        }

        public static decimal GetUpperBoundForInterpolation(decimal value, decimal desiredInterval)
        {
            if (desiredInterval <= 0) { throw new ArgumentOutOfRangeException(); }
            if (value < 0) { throw new ArgumentOutOfRangeException(); }

            decimal lowerBound = GetLowerBoundForInterpolation(value, desiredInterval);
            decimal upperBound = lowerBound + desiredInterval;

            return upperBound;
        }

        public static (decimal, decimal) GetUpperAndLowBoundsForInterpolation(decimal value, decimal desiredInterval)
        {
            if (desiredInterval <= 0) { throw new ArgumentOutOfRangeException(); }
            if (value < 0) { throw new ArgumentOutOfRangeException(); }

            decimal lowerBound = GetLowerBoundForInterpolation(value, desiredInterval);
            decimal upperBound = GetUpperBoundForInterpolation(value, desiredInterval);

            return (lowerBound, upperBound);
        }

        /// <summary>
        /// This is useful when you have two values from the POH, let's say
        /// </summary>
        /// <param name="lowerValue">The lower of two values to interpolate beteeen (e.g. landing distance of 360 for a pressure altitude of 1000)</param>z
        /// <param name="upperValue">The higher of two values to interpolate beteeen (e.g. landing distance of 440 for a pressure altitude of 1000)</param>
        /// <param name="valueForInterpolation">The value to interpolate by (e.g. actual pressure altitude is 1500)</param>z
        /// <param name="desiredInterval"> The interval at which values is provided in the underlying data (e.g. for pressure altitude, always 1000)</param>
        public static decimal Interpolate(decimal lowerValue, decimal upperValue, decimal valueForInterpolation, int desiredInterval)
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

            decimal lowerbound = GetLowerBoundForInterpolation(valueForInterpolation, desiredInterval);
            decimal upperBound = GetUpperBoundForInterpolation(valueForInterpolation, desiredInterval);

            decimal interpolationFactor = CalculateInterpolationFactor(valueForInterpolation, lowerbound, upperBound);

            decimal interpolatedValue =
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
        public static decimal InterpolateDistanceByWeight(decimal weightInterpolationFactor, decimal distance_LowerWeight, decimal distance_HigherWeight)
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