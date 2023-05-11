using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;


namespace Airborn.web.Models
{
    [Table("runways")]
    public class Runway
    {

        private Runway()
        {
        }

        private Runway(Airport airport)
        {
            Airport = airport;
        }

        public Runway(Airport airport, Direction? runwayHeading, string runwayName) : this(airport)
        {
            _runway_Heading_Magnetic = runwayHeading;
            Runway_Name = runwayName;
        }

        [NotMapped]
        public Airport Airport
        {
            get; set;
        }

        [Column("old_runway_id")]
        public int Runway_Id
        {
            get; set;
        }

        [Column("fk_airport_id")]

        public int Airport_Id
        {
            get; set;
        }

        [Display(Name = "Airport Identifier")]
        [Column("airport_ident")]
        public string Airport_Ident
        {
            get; set;
        }

        [Display(Name = "Ruyway Identifier")]
        [Column("runway_name")]
        public string Runway_Name
        {
            get; set;
        }


        [Display(Name = "Length (ft)")]
        [Column("length_ft")]
        public int? RunwayLength
        {
            get; set;
        }

        [Display(Name = "Width (ft)")]
        [Column("width_ft")]
        public int? RunwayWidth
        {
            get; set;
        }

        [Display(Name = "Surface")]
        [Column("surface")]
        public string SurfaceText
        {
            get; set;
        }

        [Display(Name = "Surface")]
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

        [Display(Name = "Runway Elevation (ft)")]
        [Column("elevation_ft")]
        public int? ElevationFt
        {
            get; set;
        }

        [Display(Name = "Displaced Threshold (ft)")]
        [Column("displaced_threshold_ft")]
        public int? DisplacedThresholdFt
        {
            get; set;
        }

        [Display(Name = "Displaced Threshold (ft)")]
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
                    return "";
                }
            }
        }

        [Display(Name = "Runway Heading (° T)")]
        [Column("heading_degt")]
        public double? HeadingDegreesTrue
        {
            get; private set;
        }

        [Display(Name = "Runway Heading (° T)")]
        [NotMapped]
        public string Runway_HeadingTrue_Formatted
        {
            get
            {
                if (!HeadingDegreesTrue.HasValue)
                {
                    return "Unknown";
                }
                return HeadingDegreesTrue.Value.ToString("#") + " °M";
            }
        }

        private Direction? _runway_Heading_Magnetic;

        [Display(Name = "Runway Heading (° M)")]
        [NotMapped]
        public Direction? Runway_Heading_Magnetic
        {
            get
            {
                // If we have a true heading, and a magnetic variation, we can calculate the magnetic heading
                if (
                    _runway_Heading_Magnetic == null
                    &&
                    HeadingDegreesTrue.HasValue
                )
                {
                    _runway_Heading_Magnetic = Direction.FromTrue(HeadingDegreesTrue.Value, Airport.MagneticVariation.Value);
                }
                // If we have a runway name, we can try to parse the runway heading from the name
                else if (_runway_Heading_Magnetic == null
                 && Runway_Name != null)
                {
                    _runway_Heading_Magnetic = GetRunwayHeadingFromRunwayName(this);
                }


                return _runway_Heading_Magnetic;
            }
        }

        [Display(Name = "Runway Heading (° M)")]
        [NotMapped]
        public string Runway_HeadingMagnetic_Formatted
        {
            get
            {
                if (!Runway_Heading_Magnetic.HasValue)
                {
                    return "Unknown";
                }
                return Runway_Heading_Magnetic.Value.DirectionMagnetic.ToString("#") + " °M";
            }
        }

        [Display(Name = "Runway Length (ft)")]
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

        [Display(Name = "Landing Available Length (ft)")]
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

        [Display(Name = "Landing Available Length (ft)")]
        [NotMapped]
        public string LandingAvailableLength_Formatted
        {
            get
            {

                return LandingAvailableLength.TotalFeet.ToString("#,##0") + " ft x " + RunwayWidth + " ft";
            }
        }

        private double? _slope;

        [Display(Name = "Slope %")]
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

        [Display(Name = "Slope %")]
        [NotMapped]
        public string Slope_Formatted
        {
            get
            {
                if (Slope.HasValue)
                {
                    return (Slope.Value / 100).ToString("P2");
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        private double? GetRunwaySlope()
        {

            if (!String.IsNullOrEmpty(Runway_Name) && Char.IsLetter(Runway_Name[0]))
            {
                return null; // don't try to figure out a runway slope for helicopters, etc
            }

            if (RunwayLength != null)
            {
                var oppositeRunway = Airport.Runways.Find(r => r.Runway_Name == GetOppositeRunwayName(Runway_Name) && r.Airport.Airport_Id == Airport.Airport_Id);

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
        public static string GetOppositeRunwayName(string runway)
        {

            string pattern = @"^\d+([RC]L|[RC]|[L])?$";

            if (!Regex.IsMatch(runway, pattern))
            {
                Console.WriteLine("Runway number is not valid");
                return null;
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

        /// <summary>
        /// Sets the calculator's runway heading for a given runway
        /// </summary>
        public Direction? GetRunwayHeadingFromRunwayName(Runway runway)
        {

            string pattern = @"^\d+([RLC])?$";
            Regex regex = new Regex(pattern);

            if (!regex.IsMatch(runway.Runway_Name))
            {
                return null;
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

        public static Runway FromMagnetic(Airport airport, int magneticHeading, string runwayName)
        {
            return new Runway(airport, Direction.FromMagnetic(magneticHeading), runwayName);
        }

        public static Runway FromMagnetic(Airport airport, int magneticHeading, int magneticVariation, string runwayName)
        {
            return new Runway(airport, Direction.FromMagnetic(magneticHeading, magneticVariation), runwayName);
        }
    }
}