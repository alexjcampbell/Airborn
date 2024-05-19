using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Airborn.web.Models
{
    [Table("continents")]
    public class Continent
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("continent_id")]
        public int Continent_Id { get; set; }

        [Display(Name = "Continent Code")]
        [Column("Continent Code")]
        public string Code
        {
            get; set;
        }

        [Display(Name = "Continent Name")]
        [Column("continent_name")]
        public string ContinentName
        {
            get; set;
        }

    }
}