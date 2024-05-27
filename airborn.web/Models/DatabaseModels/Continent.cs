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
        [Column("continent_code")]
        public string Code
        {
            get; set;
        }

        [Column("last_updated_ts")]
        public DateTime LastUpdated
        {
            get; set;
        }

        [Display(Name = "Continent Name")]
        [Column("continent_name")]
        public string ContinentName
        {
            get; set;
        }

        public virtual ICollection<Country> Countries { get; set; }

        

    }
}