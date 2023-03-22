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
        public double? RunwayLengthConverted
        {
            get{
                if(RunwayLength.Length > 0){
                    return Convert.ToDouble(RunwayLength);
                }
                return null;

            }
        }

        [NotMapped]
        public string RunwayLengthFriendly
        {
            get
            {
                if (RunwayLength.Length > 0)
                {
                    return RunwayLengthConverted?.ToString("#,##0") + " ft";
                }

                return "Unknown";
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
                return RunwayHeading.DirectionMagnetic.ToString() + " °M";
            }
        }

        public static Runway FromMagnetic(int magneticHeading, int magneticVariation)
        {
            return new Runway(Direction.FromMagnetic(magneticHeading, magneticVariation));
        }
    }
}