using System;

namespace Airborn.web.Models
{
    public struct Wind
    {
        private Direction _direction;

        private int _strengthKts;

        private int? _strengthGustsKts;

        public Direction Direction
        {
            get
            {
                return _direction;
            }
        }

        public int StrengthKts
        {
            get
            {
                return _strengthKts;
            }
        }

        private Wind(Direction direction, int strengthKts)
        {
            _direction = direction;
            _strengthKts = strengthKts;
            _strengthGustsKts = null;
        }
        private Wind(Direction direction, int strengthKts, int strengthGustsKts)
        {
            _direction = direction;
            _strengthKts = strengthKts;
            _strengthGustsKts = strengthGustsKts;
        }

        public static Wind FromMagnetic(int magneticHeading, int strengthKts)
        {
            return new Wind(Direction.FromMagnetic(magneticHeading), strengthKts);
        }

        public static Wind FromMagnetic(int magneticHeading, int magneticVariation, int strengthKts)
        {
            return new Wind(Direction.FromMagnetic(magneticHeading, magneticVariation), strengthKts);
        }

    }
}