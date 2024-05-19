using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Airborn.web.Models
{
    [Table("regions")]
    public class Region
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("region_id")]
        public int Region_Id { get; set; }

        [Column("imported_region_id")]
        public int ImportedRegion_ID { get; set; }

        [Display(Name = "Region Code")]
        [Column("region_code")]
        public string RegionCode
        {
            get; set;
        }
        
        [Display(Name = "Local Code")]
        [Column("local_code")]
        public string LocalCode
        {
            get; set;
        }

        [Display(Name = "Continent")]
        [Column("continent")]
        public string Continent
        {
            get; set;
        }

        [Display(Name = "ISO Country Code")]
        [Column("iso_country")]
        public string CountryCode
        {
            get; set;
        }


        [Display(Name = "Wikipedia Link")]
        [Column("wikipedia_link")]
        public string WikipediaLink
        {
            get; set;
        }

        [Display(Name = "Keywords")]
        [Column("keywords")]
        public string Keywords
        {
            get; set;
        }

    }
}