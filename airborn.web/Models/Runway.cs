using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airborn.web.Models
{

    public class Runway
    {
        private Runway()
        {
        }

        public Runway(Direction runwayHeading)
        {
            RunwayHeading = runwayHeading;
        }
        public string Id
        {
            get; set;
        }

        public string Airport_Ident
        {
            get; set;
        }

        [NotMapped]
        public string RunwayIdentifier
        {
            get; set;
        }        

        [Column("Le_Ident")]
        public string RunwayIdentifier_Primary
        {
            get; set;
        }

        [Column("He_Ident")]
        public string RunwayIdentifier_Secondary
        {
            get; set;
        }

        [Column("Length_Ft")]
        public string RunwayLength
        {
            get; set;
        }

        [NotMapped]
        public double RunwayLengthConverted
        {
            get{
                return Convert.ToDouble(RunwayLength);
            }
        }

        [NotMapped]
        public string RunwayLengthFriendly
        {
            get
            {
                return RunwayLengthConverted.ToString("#,##0") + "ft";
            }
        }


        // todo use or remove this
        [NotMapped]
        public Distance TakeoffAvailableLength { get; }

        [NotMapped]
        public Distance LandingAvailableLength { get; }

        [NotMapped]
        public Direction RunwayHeading { get; set; }

        public string RunwayHeadingMagneticConverted
        {
            get 
            {
                return RunwayHeading.DirectionMagnetic.ToString() + " degrees M";
            }
        }

        public static Runway FromMagnetic(int magneticHeading, int magneticVariation)
        {
            return new Runway(Direction.FromMagnetic(magneticHeading, magneticVariation));
        }
    }
}