using System;

namespace Airborn.web.Models
{
    public struct Direction : IEquatable<Direction>, IComparable<Direction>
    {
        private readonly double _directionTrue;

        private readonly double? _magneticVariation;


        /// <summary>
        /// Creates a new Direction object from a true direction in degrees
        /// (used privately because outside of the internals of this class you should never
        /// specify a true direction without specifying magnetic variation)
        /// </summary>
        private Direction(double directionTrue)
        {
            this._directionTrue = directionTrue;
        }


        public Direction(double directionTrue, double magneticVariation)
        {
            this._directionTrue = directionTrue;
            this._magneticVariation = magneticVariation;
        }


        public double DirectionTrue
        {
            get
            {
                return _directionTrue;
            }
        }

        public double DirectionMagnetic
        {
            get
            {
                return ConvertTrueToMagnetic(_directionTrue, _magneticVariation);
            }
        }

        public double? MagneticVariation
        {
            get
            {
                return _magneticVariation;
            }
        }

        public static Direction FromMagnetic(double directionMagnetic)
        {
            return new Direction(ConvertMagneticToTrue(directionMagnetic, null));
        }

        public static Direction FromMagnetic(double directionMagnetic, double magneticVariation)
        {
            return new Direction(ConvertMagneticToTrue(directionMagnetic, magneticVariation), magneticVariation);
        }

        public static Direction FromTrue(double directionTrue)
        {
            return new Direction(directionTrue);
        }

        public static Direction FromTrue(double directionTrue, double magneticVariation)
        {
            return new Direction(directionTrue, magneticVariation);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Direction))
                return false;

            return Equals((Direction)obj);
        }

        public bool Equals(Direction other)
        {
            return this._directionTrue == other._directionTrue;
        }

        public int CompareTo(Direction other)
        {
            return this._directionTrue.CompareTo(other._directionTrue);
        }

        public override int GetHashCode()
        {
            return _directionTrue.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}[m]", DirectionTrue);
        }

        /// <summary>
        /// We compute the magnetic direction from a true direction by adding the magnetic variation
        /// from the true direction. (Magnetic variation will be positive if east, negative if west)
        /// </summary>
        public static double ConvertMagneticToTrue(double trueHeading, double? magneticVariation)
        {
            double magneticDirection = trueHeading;

            if (magneticVariation > 0)
            {
                magneticDirection = trueHeading + magneticVariation.Value;
            }
            else if (magneticVariation < 0)
            {
                magneticDirection = trueHeading - magneticVariation.Value;
            }
            else
            {
                magneticDirection = trueHeading;
            }

            if (magneticDirection < 0)
            {
                magneticDirection += 360;
            }
            else if (magneticDirection >= 360)
            {
                magneticDirection -= 360;
            }

            return magneticDirection;
        }

        /// <summary>
        /// We compute the true direction from a magnetic direction by subtracting the magnetic variation
        /// to the magnetic direction. (Magnetic variation will be positive if east, negative if west)
        /// </summary>
        public static double ConvertTrueToMagnetic(double trueHeading, double? magneticVariation)
        {
            if (!magneticVariation.HasValue)
            {
                return trueHeading;
            }

            double magneticDirection = trueHeading;

            if (magneticVariation > 0)
            {
                magneticDirection = trueHeading - magneticVariation.Value;
            }
            else if (magneticVariation < 0)
            {
                magneticDirection = trueHeading + magneticVariation.Value;
            }
            else
            {
                magneticDirection = trueHeading;
            }

            if (magneticDirection < 0)
            {
                magneticDirection += 360;
            }
            else if (magneticDirection >= 360)
            {
                magneticDirection -= 360;
            }

            return magneticDirection;

        }

        /// <summary>
        /// Converting from true to magnetic (or magnetic to true) can result in a negative direction
        /// e.g. a direction of 10 degrees magnetic with magnetic variation of 20 degrees west results in
        /// a direction of -10. To fix this, we add 360
        /// </summary>
        public static int ReturnPositiveDirectionIfNegative(int direction)
        {
            if (direction < 0)
            {
                return direction + 360;
            }
            else
            {
                return direction;
            }
        }
    }
}
