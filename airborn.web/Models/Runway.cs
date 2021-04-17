using System;

namespace Airborn.web.Models
{
    public class Runway 
    {
        private Runway()
        {
        }

        public Runway (Direction runwayHeading)
        {
            RunwayHeading = runwayHeading;
        }

        public Distance TakeoffAvailableLength {get;}

        public Distance LandingAvailableLength {get;}

        public Direction RunwayHeading {get;set;}
    }
}