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

        public int? StrengthGustsKts
        {
            get
            {
                return _strengthGustsKts;
            }
        }

        public Wind(Direction direction, int strengthKts)
        {
            _direction = direction;
            _strengthKts = strengthKts;
            _strengthGustsKts = null;
        }
        public Wind(Direction direction, int strengthKts, int strengthGustsKts)
        {
            _direction = direction;
            _strengthKts = strengthKts;
            _strengthGustsKts = strengthGustsKts;
        }
    }
}