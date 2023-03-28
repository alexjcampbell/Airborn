using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Airborn.web.Models
{
    [Table("Runways")]
    public class Runway
    {
        private Runway()
        {
        }

        public Runway(Direction runwayHeading)
        {
            RunwayHeading = runwayHeading;
        }

        public int Runway_Id
        {
            get; set;
        }

        public string Airport_Ident
        {
            get; set;
        }
        
        public string Runway_Name
        {
            get; set;
        }


        [Column("Length_Ft")]
        public string RunwayLength
        {
            get; set;
        }

        [Column("Width_Ft")]
        public string RunwayWidth
        {
            get; set;
        }

        [Column("Surface")]
        public string SurfaceText
        {
            get; set;
        }

        [Column("Displayed_Threshold_Ft")]
        public string DisplayedThresholdFt
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