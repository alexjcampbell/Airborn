using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Airborn.web.Models
{
    [Table("countries")]
    public class Country
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("country_id")]
        public int Country_Id { get; set; }

        [Column("last_updated_ts")]
        public DateTime LastUpdated
        {
            get; set;
        }

        [Column("imported_country_id")]
        public int ImportedCountry_Id { get; set; }        

        [ForeignKey("fk_continent_id")]
        public int Continent_Id
        {
            get; set;
        }

        public virtual Continent Continent
        {
            get; set;
        }


        [Display(Name = "Country Name")]
        [Column("country_name")]
        public string CountryName
        {
            get; set;
        }

        [Display(Name = "Country Code")]
        [Column("country_code")]
        public string CountryCode
        {
            get; set;
        }

        [Display(Name = "Continent Code")]
        [Column("continent_code")]
        public string ContinentCode
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

        [Column("slug")]
        public string Slug
        {
            get; set;
        }

        public virtual ICollection<Airport> Airports { get; set; }

        public virtual ICollection<Region> Regions { get; set; }

    }
}