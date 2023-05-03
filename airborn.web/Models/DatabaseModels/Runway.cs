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
        public int? RunwayLength
        {
            get; set;
        }

        [Column("Width_Ft")]
        public int? RunwayWidth
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

        [Column("Elevation_Ft")]
        public int? ElevationFt
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
        public int? DisplacedThresholdFt
        {
            get; set;
        }

        [NotMapped]
        public string DisplacedThreshold_Formatted
        {
            get
            {
                if (DisplacedThresholdFt.HasValue)
                {
                    return DisplacedThresholdFt?.ToString("#,##0") + " ft";
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        [NotMapped]
        public string RunwayLengthFriendly
        {
            get
            {
                string runwayInformation;

                if (RunwayLength.HasValue)
                {
                    if (RunwayWidth.HasValue)
                    {
                        runwayInformation = RunwayLength?.ToString("#,##0") + " ft x " + RunwayWidth + " ft";
                    }
                    else
                    {
                        runwayInformation = RunwayLength?.ToString("#,##0") + " ft";
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
                if (RunwayLength.HasValue && DisplacedThresholdFt.HasValue)
                {
                    return Distance.FromFeet(RunwayLength.Value - DisplacedThresholdFt.Value);
                }
                else if (RunwayLength != null)
                {
                    return Distance.FromFeet(RunwayLength.Value);
                }
                else
                {
                    return new Distance(0);
                }
            }
        }

        [NotMapped]
        public string LandingAvailableLength_Formatted
        {
            get
            {

                return LandingAvailableLength.TotalFeet.ToString("#,##0") + " ft x " + RunwayWidth + " ft";
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

        [NotMapped]
        public string RunwayHeadingMagneticConverted
        {
            get
            {
                return RunwayHeading.DirectionMagnetic.ToString() + " °M";
            }
        }

        [NotMapped]
        public decimal? Slope
        {
            get; set;
        }

        /// <summary>
        /// Gets the opposite runway for a given runway
        /// </summary>
        public static string GetOppositeRunway(string runway)
        {
            if (string.IsNullOrEmpty(runway))
            {
                throw new ArgumentException("Runway cannot be null or empty.");
            }

            bool hasLetter = false;
            char letter = ' ';
            string runwayNumber = runway;

            if (char.IsLetter(runway[runway.Length - 1]))
            {
                hasLetter = true;
                letter = runway[runway.Length - 1];
                runwayNumber = runway.Substring(0, runway.Length - 1);
            }

            if (!int.TryParse(runwayNumber, out int number))
            {
                throw new ArgumentException("Invalid runway number format.");
            }

            int oppositeNumber = (number + 18) % 36;
            if (oppositeNumber == 0)
            {
                oppositeNumber = 36;
            }

            string oppositeRunway = oppositeNumber.ToString();

            if (hasLetter)
            {
                switch (letter)
                {
                    case 'L':
                        letter = 'R';
                        break;
                    case 'R':
                        letter = 'L';
                        break;
                    case 'C':
                        // Keep the letter 'C' for the opposite runway
                        break;
                    default:
                        throw new ArgumentException("Invalid runway letter. Only L, R, and C are allowed.");
                }

                oppositeRunway += letter;
            }

            return oppositeRunway;
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

        public static double CalculateSlope(double startingElevation, double endingElevation, double distance)
        {
            if (distance <= 0)
            {
                throw new ArgumentException("Runway length must be greater than zero.");
            }

            double elevationDifference = endingElevation - startingElevation;
            double slope = (elevationDifference / distance) * 100;

            return slope;
        }
    }
}