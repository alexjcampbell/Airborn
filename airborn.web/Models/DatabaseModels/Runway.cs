using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Airborn.web.Models
{
    [Table("Runways")]
    public class Runway
    {

        public Runway()
        {
        }

        public Runway(string runwayName)
        {
            Runway_Name = runwayName;
        }

        public Runway(Direction runwayHeading)
        {
            _runwayHeading = runwayHeading;
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

        [Column("Surface_Friendly")]
        public string Surface_Friendly
        {
            get; set;
        }

        [NotMapped]
        public string Surface_Friendly_Output
        {
            get
            {
                if (Surface_Friendly?.Length > 0)
                {
                    return Surface_Friendly;
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        [Column("Displaced_Threshold_Ft")]
        public string DisplacedThresholdFt
        {
            get; set;
        }

        [NotMapped]
        public decimal? RunwayLengthConverted
        {
            get
            {
                if (RunwayLength.Length > 0)
                {
                    return Convert.ToDecimal(RunwayLength);
                }
                return null;

            }
        }

        [NotMapped]
        public decimal? DisplacedThresholdConverted
        {
            get
            {
                if (DisplacedThresholdFt.Length > 0)
                {
                    return Convert.ToDecimal(DisplacedThresholdFt);
                }
                return null;

            }
        }

        [NotMapped]
        public string RunwayLengthFriendly
        {
            get
            {
                string runwayInformation;

                if (RunwayLength.Length > 0)
                {
                    if (RunwayWidth.Length > 0)
                    {
                        runwayInformation = RunwayLengthConverted?.ToString("#,##0") + " ft x " + RunwayWidth + " ft";
                    }
                    else
                    {
                        runwayInformation = RunwayLengthConverted?.ToString("#,##0") + " ft";
                    }
                }
                else
                {
                    runwayInformation = "Unknown";
                }


                return runwayInformation;
            }
        }

        [NotMapped]
        public Distance LandingAvailableLength
        {
            get
            {
                if (RunwayLengthConverted != null && DisplacedThresholdConverted != null)
                {
                    return Distance.FromFeet(RunwayLengthConverted.Value - DisplacedThresholdConverted.Value);
                }
                else if (RunwayLengthConverted != null)
                {
                    return Distance.FromFeet(RunwayLengthConverted.Value);
                }
                else
                {
                    return new Distance(0);
                }
            }
        }

        private Direction? _runwayHeading;

        [NotMapped]
        public Direction RunwayHeading
        {
            get
            {
                if (_runwayHeading.HasValue)
                {
                    return _runwayHeading.Value;
                }
                else
                {
                    return GetRunwayHeading(this);
                }
            }
        }


        public string RunwayHeadingMagneticConverted
        {
            get
            {
                return RunwayHeading.DirectionMagnetic.ToString() + " °M";
            }
        }

        public static Runway FromMagnetic(int magneticHeading)
        {
            return new Runway(Direction.FromMagnetic(magneticHeading));
        }

        public static Runway FromMagnetic(int magneticHeading, int magneticVariation)
        {
            return new Runway(Direction.FromMagnetic(magneticHeading, magneticVariation));
        }

        /// <summary>
        /// Sets the calculator's runway heading for a given runway
        /// </summary>
        private static Direction GetRunwayHeading(Runway runway)
        {
            // RunwayIdentifer could be like 10R or 28L so we use a regex to get only the first two characters
            string runwayName = Regex.Replace(runway.Runway_Name, @"\D+", "");

            // runway heading is the runway name (e.g. 10 or 28) multiplied by 10
            return new Direction(
                    int.Parse(runwayName) * 10,
                    0
                    );
        }
    }
}