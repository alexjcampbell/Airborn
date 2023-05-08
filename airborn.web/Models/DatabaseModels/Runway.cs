using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Airborn.web.Models
{
    [Table("Runways")]
    public class Runway
    {

        private Runway()
        {
        }

        public Runway(Airport airport)
        {
            Airport = airport;
        }

        public Runway(Airport airport, string runwayName) : this(airport)
        {
            Runway_Name = runwayName;
        }

        public Runway(Airport airport, Direction runwayHeading) : this(airport)
        {
            _runwayHeading = runwayHeading;
        }

        [NotMapped]
        public Airport Airport
        {
            get; set;
        }

        public int Runway_Id
        {
            get; set;
        }

        [Column("fk_Airport_Id")]

        public int Airport_Id
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

        [NotMapped]
        public RunwaySurface Surface
        {
            get
            {
                if (SurfaceText?.Length > 0)
                {
                    return ParseRunwaySurface(SurfaceText);
                }
                else
                {
                    return RunwaySurface.Unknown;
                }
            }
        }

        [Column("Elevation_Ft")]
        public int? ElevationFt
        {
            get; set;
        }

        [Column("Displaced_Threshold_Ft")]
        public int? DisplacedThresholdFt
        {
            get; set;
        }

        [Column("heading_degT")]
        public double? HeadingDegreesTrue
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
                return RunwayHeading.DirectionMagnetic.ToString("#") + " °M";
            }
        }

        private double? _slope;

        [NotMapped]
        public double? Slope
        {
            get
            {
                if (!_slope.HasValue)
                {
                    return GetRunwaySlope();
                }
                return _slope;
            }
            private set
            {
                _slope = value;
            }
        }

        private double? GetRunwaySlope()
        {

            if (Runway_Name.StartsWith("H"))
            {
                return null; // don't try to figure out a runway slope for helicopters
            }

            if (RunwayLength != null)
            {
                var oppositeRunway = Airport.Runways.Find(r => r.Runway_Name == GetOppositeRunway(Runway_Name) && r.Airport.Ident == Airport.Ident.ToUpper());

                if (oppositeRunway == null || oppositeRunway.ElevationFt == null)
                {
                    return null;
                }


                else if (ElevationFt.HasValue && oppositeRunway.ElevationFt.HasValue && RunwayLength.HasValue)
                {
                    return Runway.CalculateSlope(
                        ElevationFt.Value,
                        oppositeRunway.ElevationFt.Value,
                        RunwayLength.Value);
                }

            }

            return null;
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

            letter = char.ToUpper(letter);

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

        public static Runway FromMagnetic(Airport airport, int magneticHeading)
        {
            return new Runway(airport, Direction.FromMagnetic(magneticHeading));
        }

        public static Runway FromMagnetic(Airport airport, int magneticHeading, int magneticVariation)
        {
            return new Runway(airport, Direction.FromMagnetic(magneticHeading, magneticVariation));
        }

        /// <summary>
        /// Sets the calculator's runway heading for a given runway
        /// </summary>
        private static Direction GetRunwayHeading(Runway runway)
        {
            if (runway.Airport != null && runway.Airport.MagneticVariation.HasValue)
            {
                return Direction.FromTrue(runway.HeadingDegreesTrue.Value, runway.Airport.MagneticVariation.Value);
            }
            else
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

        public static RunwaySurface ParseRunwaySurface(string code)
        {
            code = code.Trim().ToUpperInvariant();

            return code switch
            {
                "CON" or "CONCRETE" or "PEM" or "ASP" or "ASPHALT" or "ASPH" or
                "ASPH-G" or "ASPH-CONC-F" or "ASPH-F" or "CONC-G" or "CONC-F" or
                "GVL"
                    => RunwaySurface.Paved,
                "TURF-G" or "GRS" or "GRASS" or "GRASSED BROWN CLAY" or "TURF" => RunwaySurface.Grass,
                _ => RunwaySurface.Unknown,
            };
        }
    }
}