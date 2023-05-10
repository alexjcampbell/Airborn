using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airborn.web.Models
{
    [Table("airports")]
    public class Airport
    {
        [Column("old_airport_id")]
        public int Airport_Id
        {
            get; set;
        }

        [Display(Name = "Airport Identifier")]
        [Column("ident")]
        public string Ident
        {
            get; set;
        }

        [Display(Name = "Field elevation (ft)")]
        [Column("elevation_ft")]
        public int? FieldElevation
        {
            get; set;
        }

        [Display(Name = "Field elevation (ft)")]
        public Distance FieldElevationAsDistance
        {
            get
            {
                return Distance.FromFeet(FieldElevation.Value);
            }
        }

        [Display(Name = "Airport Type")]
        [Column("type")]
        public string Type
        {
            get; set;
        }

        [Display(Name = "Airport Type")]
        [NotMapped]
        public string Type_Friendly
        {
            get
            {
                return AirportTypeExtensions.GetDisplayName(
                    (AirportType)Enum.Parse(typeof(AirportType), Type)
                    );
            }
        }

        [Display(Name = "Airport Name")]
        [Column("name")]
        public string Name
        {
            get; set;
        }

        [Display(Name = "Latitude")]
        [Column("latitude_deg")]
        public double? Latitude_Deg
        {
            get; set;
        }

        [Display(Name = "Longitude")]
        [Column("longitude_deg")]
        public double? Longitude_Deg
        {
            get; set;
        }

        [Display(Name = "Magnetic Variation")]
        [Column("magnetic_variation")]
        public double? MagneticVariation
        {
            get; set;
        }

        public List<Runway> Runways
        {
            get;
            set;
        }

        [NotMapped]
        public List<Runway> UsableRunways
        {
            get
            {
                return Runways.FindAll(
                    r => r.Runway_Name != null
                    && (!r.Runway_Name.StartsWith("H")) // remove helipads"
                    );
            }
        }

    }
}