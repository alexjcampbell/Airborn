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
        public static decimal CalculateInterpolationFactor(int value, int lowerBound, int upperBound)
        {
            if (value < 0) { throw new ArgumentOutOfRangeException(value.ToString()); }
            if (lowerBound >= upperBound) { throw new ArgumentOutOfRangeException(); }
            if (value < lowerBound) { throw new ArgumentOutOfRangeException(); }
            if (value > upperBound) { throw new ArgumentOutOfRangeException(); }

            decimal interpolationFactor = (decimal)(value - lowerBound) / (decimal)(upperBound - lowerBound);

            return interpolationFactor;
        }

        public static int GetLowerBoundForInterpolation(int value, int desiredInterval)
        {
            if (desiredInterval <= 0) { throw new ArgumentOutOfRangeException(); }
            if (value < 0) { throw new ArgumentOutOfRangeException(); }

            int lowerBound = value - (value % desiredInterval);

            return lowerBound;
        }

        public static int GetUpperBoundForInterpolation(int value, int desiredInterval)
        {
            if (desiredInterval <= 0) { throw new ArgumentOutOfRangeException(); }
            if (value < 0) { throw new ArgumentOutOfRangeException(); }

            int lowerBound = value - (value % desiredInterval);
            int upperBound = lowerBound + desiredInterval;

            return upperBound;
        }

        public static (int, int) GetUpperAndLowBoundsForInterpolation(int value, int desiredInterval)
        {
            if (desiredInterval <= 0) { throw new ArgumentOutOfRangeException(); }
            if (value < 0) { throw new ArgumentOutOfRangeException(); }

            return (GetLowerBoundForInterpolation(value, desiredInterval), GetUpperBoundForInterpolation(value, desiredInterval));
        }


        public static decimal Interpolate(decimal lowerValue, decimal upperValue, int valueForInterpolation, int desiredInterval)
        {

            if (valueForInterpolation < 0) { throw new ArgumentOutOfRangeException(valueForInterpolation.ToString()); }
            if (valueForInterpolation < GetLowerBoundForInterpolation(valueForInterpolation, desiredInterval)) { throw new ArgumentOutOfRangeException(); }
            if (valueForInterpolation > GetUpperBoundForInterpolation(valueForInterpolation, desiredInterval)) { throw new ArgumentOutOfRangeException(); }
            if (desiredInterval <= 0) { throw new ArgumentOutOfRangeException(); }

            if (valueForInterpolation == lowerValue)
            {
                return lowerValue;
            }

            int lowerInterpolation = GetLowerBoundForInterpolation(valueForInterpolation, desiredInterval);
            int upperInterpolation = GetUpperBoundForInterpolation(valueForInterpolation, desiredInterval);

            decimal interpolationFactor = CalculateInterpolationFactor(valueForInterpolation, lowerInterpolation, upperInterpolation);

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