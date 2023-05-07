using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airborn.web.Models
{
    [Table("airports")]
    public class Airport
    {
        public string Id
        {
            get; set;
        }

        public string Ident
        {
            get; set;
        }

        [Column("Elevation_Ft")]
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

        public string Type
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public double? Latitude_Deg
        {
            get; set;
        }

        public double? Longitude_Deg
        {
            get; set;
        }

        [Column("Magnetic_Variation")]
        public double? MagneticVariation
        {
            get; set;
        }

    }
}