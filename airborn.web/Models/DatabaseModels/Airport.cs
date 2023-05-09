using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

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

        [Column("ident")]
        public string Ident
        {
            get; set;
        }

        [Column("elevation_ft")]
        public int? FieldElevation
        {
            get; set;
        }

        public Distance FieldElevationAsDistance
        {
            get
            {
                return Distance.FromFeet(FieldElevation.Value);
            }
        }

        [Column("type")]
        public string Type
        {
            get; set;
        }

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

        [Column("name")]
        public string Name
        {
            get; set;
        }

        [Column("latitude_deg")]
        public double? Latitude_Deg
        {
            get; set;
        }

        [Column("longitude_deg")]
        public double? Longitude_Deg
        {
            get; set;
        }

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