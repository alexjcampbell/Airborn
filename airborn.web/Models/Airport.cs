using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airborn.web.Models
{
    [Table("airports")]
    public class Airport
    {
        public string Id 
        {
            get;set;
        }

        public string Ident 
        {
            get;set;
        }

        [Column("Elevation_Ft")]
        public string FieldElevation {
            get; set;
        }

    }
}