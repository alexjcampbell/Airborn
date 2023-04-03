using System;

namespace Airborn.web.Models
{
    public struct Direction : IEquatable<Direction>, IComparable<Direction>
    {
        private readonly int _directionTrue;

        private readonly int? _magneticVariation;

        public Direction(int directionTrue)
        {
            this._directionTrue = directionTrue;
        }


        public Direction(int directionTrue, int magneticVariation)
        {
            this._directionTrue = directionTrue;
            this._magneticVariation = magneticVariation;
        }


        public int DirectionTrue
        {
            get
            {
                return _directionTrue;
            }
        }

        public int DirectionMagnetic
        {
            get
            {
                return ConvertTrueToMagnetic(_directionTrue, _magneticVariation);
            }
        }

        public int? MagneticVariation
        {
            get
            {
                return _magneticVariation;
            }
        }

        public static Direction FromMagnetic(int directionMagnetic)
        {
            return new Direction(ConvertMagneticToTrue(directionMagnetic, null));
        }

        public static Direction FromMagnetic(int directionMagnetic, int magneticVariation)
        {
            return new Direction(ConvertMagneticToTrue(directionMagnetic, magneticVariation), magneticVariation);
        }

        public static Direction FromTrue(int directionTrue)
        {
            return new Direction(directionTrue);
        }

        public static Direction FromTrue(int directionTrue, int magneticVariation)
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
        /// We compute the magnetic direction from a true direction by subtracting the magnetic variation
        /// from the true direction. (Magnetic variation will be positive if east, negative if west)
        /// </summary>
        private static int ConvertMagneticToTrue(int directionMagnetic, int? magneticVariation)
        {

            if (magneticVariation.HasValue)
            {
                directionMagnetic -= magneticVariation.Value;
            }

            // modulo by 360 to make sure that we never end up with directions greater than 360 degrees
            int directionTrue = directionMagnetic % 360;

            return ReturnPositiveDirectionIfNegative(directionTrue);

        }

        /// <summary>
        /// We compute the true direction from a magnetic direction by adding the magnetic variation
        /// to the magnetic direction. (Magnetic variation will be positive if east, negative if west)
        /// </summary>
        private static int ConvertTrueToMagnetic(int directionTrue, int? magneticVariation)
        {

            if (magneticVariation.HasValue)
            {
                directionTrue += magneticVariation.Value;
            }

            // modulo by 360 to make sure that we never end up with directions greater than 360 degrees
            int directionMagnetic = directionTrue % 360;

            return ReturnPositiveDirectionIfNegative(directionMagnetic);

        }

        /// <summary>
        /// Converting from true to magnetic (or magnetic to true) can result in a negative direction
        /// e.g. a direction of 10 degrees magnetic with magnetic variation of 20 degrees west results in
        /// a direction of -10. To fix this, we add 360
        /// </summary>
        private static int ReturnPositiveDirectionIfNegative(int direction)
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
