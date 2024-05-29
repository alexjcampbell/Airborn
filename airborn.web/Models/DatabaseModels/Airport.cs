using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace Airborn.web.Models
{
    [Table("airports")]
    public class Airport
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("airport_id")]
        public int Airport_Id { get; set; }

        [Column("last_updated_ts")]
        public DateTime LastUpdated
        {
            get; set;
        }

        [Column("magvar_last_updated_ts")]
        public DateTime MagneticVariationLastUpdated
        {
            get; set;
        }

        [Column("imported_airport_id")]
        public int ImportedAirport_Id { get; set; }


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
        [NotMapped]
        public Distance? FieldElevationAsDistance
        {
            get
            {
                return FieldElevation.HasValue ? new Distance(FieldElevation.Value) : null;
            }
        }

        [Display(Name = "Field elevation (ft)")]
        [NotMapped]
        public string FieldElevation_Formatted
        {
            get
            {
                if (FieldElevation.HasValue)
                {
                    return FieldElevationAsDistance.Value.TotalFeet.ToString("N0") + " ft";
                }

                return null;
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

        [Display(Name = "Location")]
        [Column("municipality")]
        public string Location
        {
            get; set;
        }

        [Display(Name = "Region")]
        [Column("iso_region")]
        public string RegionCode
        {
            get; set;
        }

        [Column("fk_region_id")]
        public int Region_Id
        {
            get; set;
        }
        
        public virtual Region Region
        {
            get; set;
        }

        [Display(Name = "Country")]
        [Column("iso_country")]
        public string CountryCode
        {
            get; set;
        }

        [Column("fk_country_id")]
        public int Country_Id { get; set; }

        public virtual Country Country
        {
            get; set;
        }

        [Display(Name = "Country")]
        [NotMapped]
        public string Country_Formatted
        {
            get
            {
                if (CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .Select(c => new RegionInfo(c.Name))
                    .Any(r => r.TwoLetterISORegionName == CountryCode))
                {
                    try
                    {
                        RegionInfo region = new RegionInfo(CountryCode);

                        return region.EnglishName;
                    }
                    catch (ArgumentException)
                    {
                        return CountryCode;
                    }
                }

                return null;
            }
        }

        [Display(Name = "Scheduled Service")]
        [Column("scheduled_service")]
        public string ScheduledService
        {
            get; set;
        }

        [Display(Name = "GPS Code")]
        [Column("gps_code")]
        public string GPSCode
        {
            get; set;
        }

        [Display(Name = "IATA Code")]
        [Column("iata_code")]
        public string IATACode
        {
            get; set;
        }

        [Display(Name = "Local Code")]
        [Column("local_code")]
        public string LocalCode
        {
            get; set;
        }

        [Display(Name = "Home Link")]
        [Column("home_link")]
        public string HomeLink
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
                if(Runways == null)
                {
                    return new List<Runway>();
                }

                return Runways.FindAll(
                    r => r.Runway_Name != null
                    && (!r.Runway_Name.StartsWith("H")) // remove helipads"
                    );
            }
        }

    }
}