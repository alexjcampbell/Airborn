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

        [Column("imported_country_id")]
        public int ImportedCountry_Id { get; set; }        

        [Display(Name = "Country Name")]
        [Column("country_name")]
        public string Ident
        {
            get; set;
        }

        [Display(Name = "Country Code")]
        [Column("country_code")]
        public string CountryCode
        {
            get; set;
        }

        [Display(Name = "Continent")]
        [Column("continent")]
        public string Continent
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