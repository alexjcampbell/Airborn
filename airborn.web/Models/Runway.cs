using System;

namespace Airborn.web.Models
{
    public class Runway 
    {
        private Runway()
        {
        }

        private Runway (Direction runwayHeading)
        {
            RunwayHeading = runwayHeading;
        }

        public Distance TakeoffAvailableLength {get;}

        public Distance LandingAvailableLength {get;}

        public Direction RunwayHeading {get;set;}

        public static Runway FromMagnetic(int magneticHeading, int magneticVariation)
        {
            return new Runway(Direction.FromMagnetic(magneticHeading,magneticVariation));
        }
    }
}