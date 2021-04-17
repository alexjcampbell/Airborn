using System;

namespace Airborn.web.Models
{
    public struct Direction : IEquatable<Direction>, IComparable<Direction>
    {
        private readonly int _directionTrue;
        
        private readonly int _magneticVariation;

        private Direction (int directionTrue, int magneticVariation)
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
                // modulo by 360 to make sure that we never exceed 360 degrees
                return ConvertTrueToMagnetic(_directionTrue, _magneticVariation);
            }
        }

        public int MagneticVariation
        {
            get
            {
                return _magneticVariation;
            }
        }

        public static Direction FromMagnetic(int directionMagnetic, int magneticVariation)
        {
            return new Direction(ConvertMagneticToTrue(directionMagnetic, magneticVariation), magneticVariation);
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

        private static int ConvertMagneticToTrue(int directionMagnetic, int magneticVariation)
        {
        
            // modulo by 360 to make sure that we never end up with directions greater than 360 degrees
            int directionTrue = (directionMagnetic - magneticVariation) % 360;

            if(directionTrue < 0) 
            { 
                return directionTrue + 360;
            }
            else
            {
                return directionTrue;
            }

        }

        private static int ConvertTrueToMagnetic(int directionTrue, int magneticVariation)
        {
        
            // modulo by 360 to make sure that we never end up with directions greater than 360 degrees
            int directionMagnetic = (directionTrue + magneticVariation) % 360;

            if(directionMagnetic < 0) 
            { 
                return directionMagnetic + 360;
            }
            else
            {
                return directionMagnetic;
            }

        }
    }
}
