﻿// <auto-generated />
using System;
using Airborn.web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace airborn.web.Migrations
{
    [DbContext(typeof(AirbornDbContext))]
    [Migration("20240529011526_AddSlug")]
    partial class AddSlug
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Airborn.web.Models.Airport", b =>
                {
                    b.Property<int>("Airport_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("airport_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Airport_Id"));

                    b.Property<string>("CountryCode")
                        .HasColumnType("text")
                        .HasColumnName("iso_country");

                    b.Property<int>("Country_Id")
                        .HasColumnType("integer")
                        .HasColumnName("fk_country_id");

                    b.Property<int?>("FieldElevation")
                        .HasColumnType("integer")
                        .HasColumnName("elevation_ft");

                    b.Property<string>("GPSCode")
                        .HasColumnType("text")
                        .HasColumnName("gps_code");

                    b.Property<string>("HomeLink")
                        .HasColumnType("text")
                        .HasColumnName("home_link");

                    b.Property<string>("IATACode")
                        .HasColumnType("text")
                        .HasColumnName("iata_code");

                    b.Property<string>("Ident")
                        .HasColumnType("text")
                        .HasColumnName("ident");

                    b.Property<int>("ImportedAirport_Id")
                        .HasColumnType("integer")
                        .HasColumnName("imported_airport_id");

                    b.Property<string>("Keywords")
                        .HasColumnType("text")
                        .HasColumnName("keywords");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_ts");

                    b.Property<double?>("Latitude_Deg")
                        .HasColumnType("double precision")
                        .HasColumnName("latitude_deg");

                    b.Property<string>("LocalCode")
                        .HasColumnType("text")
                        .HasColumnName("local_code");

                    b.Property<string>("Location")
                        .HasColumnType("text")
                        .HasColumnName("municipality");

                    b.Property<double?>("Longitude_Deg")
                        .HasColumnType("double precision")
                        .HasColumnName("longitude_deg");

                    b.Property<double?>("MagneticVariation")
                        .HasColumnType("double precision")
                        .HasColumnName("magnetic_variation");

                    b.Property<DateTime>("MagneticVariationLastUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("magvar_last_updated_ts");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("RegionCode")
                        .HasColumnType("text")
                        .HasColumnName("iso_region");

                    b.Property<int>("Region_Id")
                        .HasColumnType("integer")
                        .HasColumnName("fk_region_id");

                    b.Property<string>("ScheduledService")
                        .HasColumnType("text")
                        .HasColumnName("scheduled_service");

                    b.Property<string>("Type")
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.Property<string>("WikipediaLink")
                        .HasColumnType("text")
                        .HasColumnName("wikipedia_link");

                    b.HasKey("Airport_Id")
                        .HasName("PK_Airport_Id");

                    b.HasIndex("Country_Id");

                    b.HasIndex("Ident")
                        .IsUnique()
                        .HasDatabaseName("IX_Airport_Ident");

                    b.HasIndex("Region_Id");

                    b.HasIndex("Type")
                        .HasDatabaseName("IX_Airport_Type");

                    b.ToTable("airports");
                });

            modelBuilder.Entity("Airborn.web.Models.Continent", b =>
                {
                    b.Property<int>("Continent_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("continent_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Continent_Id"));

                    b.Property<string>("Code")
                        .HasColumnType("text")
                        .HasColumnName("continent_code");

                    b.Property<string>("ContinentName")
                        .HasColumnType("text")
                        .HasColumnName("continent_name");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_ts");

                    b.Property<string>("Slug")
                        .HasColumnType("text")
                        .HasColumnName("slug");

                    b.HasKey("Continent_Id")
                        .HasName("PK_Continent_Id");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("continents");
                });

            modelBuilder.Entity("Airborn.web.Models.Country", b =>
                {
                    b.Property<int>("Country_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("country_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Country_Id"));

                    b.Property<string>("ContinentCode")
                        .HasColumnType("text")
                        .HasColumnName("continent_code");

                    b.Property<int>("Continent_Id")
                        .HasColumnType("integer");

                    b.Property<string>("CountryCode")
                        .HasColumnType("text")
                        .HasColumnName("country_code");

                    b.Property<string>("CountryName")
                        .HasColumnType("text")
                        .HasColumnName("country_name");

                    b.Property<int>("ImportedCountry_Id")
                        .HasColumnType("integer")
                        .HasColumnName("imported_country_id");

                    b.Property<string>("Keywords")
                        .HasColumnType("text")
                        .HasColumnName("keywords");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_ts");

                    b.Property<string>("Slug")
                        .HasColumnType("text")
                        .HasColumnName("slug");

                    b.Property<string>("WikipediaLink")
                        .HasColumnType("text")
                        .HasColumnName("wikipedia_link");

                    b.HasKey("Country_Id")
                        .HasName("PK_Country_Id");

                    b.HasIndex("Continent_Id");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("countries");
                });

            modelBuilder.Entity("Airborn.web.Models.Region", b =>
                {
                    b.Property<int>("Region_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("region_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Region_Id"));

                    b.Property<string>("CountryCode")
                        .HasColumnType("text")
                        .HasColumnName("iso_country");

                    b.Property<int>("Country_Id")
                        .HasColumnType("integer")
                        .HasColumnName("fk_country_id");

                    b.Property<int>("ImportedRegion_ID")
                        .HasColumnType("integer")
                        .HasColumnName("imported_region_id");

                    b.Property<string>("Keywords")
                        .HasColumnType("text")
                        .HasColumnName("keywords");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_ts");

                    b.Property<string>("LocalCode")
                        .HasColumnType("text")
                        .HasColumnName("local_code");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("RegionCode")
                        .HasColumnType("text")
                        .HasColumnName("region_code");

                    b.Property<string>("Slug")
                        .HasColumnType("text")
                        .HasColumnName("slug");

                    b.Property<string>("WikipediaLink")
                        .HasColumnType("text")
                        .HasColumnName("wikipedia_link");

                    b.Property<int?>("fk_country_id")
                        .HasColumnType("integer");

                    b.HasKey("Region_Id")
                        .HasName("PK_Region_Id");

                    b.HasIndex("Country_Id");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("regions", t =>
                        {
                            t.Property("fk_country_id")
                                .HasColumnName("fk_country_id1");
                        });
                });

            modelBuilder.Entity("Airborn.web.Models.Runway", b =>
                {
                    b.Property<int>("Runway_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("runway_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Runway_Id"));

                    b.Property<int>("Airport_Id")
                        .HasColumnType("integer")
                        .HasColumnName("fk_airport_id");

                    b.Property<string>("Airport_Ident")
                        .HasColumnType("text")
                        .HasColumnName("airport_ident");

                    b.Property<string>("Closed")
                        .HasColumnType("text")
                        .HasColumnName("closed");

                    b.Property<int?>("DisplacedThresholdFt")
                        .HasColumnType("integer")
                        .HasColumnName("displaced_threshold_ft");

                    b.Property<int?>("ElevationFt")
                        .HasColumnType("integer")
                        .HasColumnName("elevation_ft");

                    b.Property<double?>("HeadingDegreesTrue")
                        .HasColumnType("double precision")
                        .HasColumnName("heading_degt");

                    b.Property<int>("ImportedAirport_ID")
                        .HasColumnType("integer")
                        .HasColumnName("imported_airport_id");

                    b.Property<int>("ImportedRunway_Id")
                        .HasColumnType("integer")
                        .HasColumnName("imported_runway_id");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_ts");

                    b.Property<double?>("Latitude_Deg")
                        .HasColumnType("double precision")
                        .HasColumnName("latitude_deg");

                    b.Property<string>("Lighted")
                        .HasColumnType("text")
                        .HasColumnName("lighted");

                    b.Property<double?>("Longitude_Deg")
                        .HasColumnType("double precision")
                        .HasColumnName("longitude_deg");

                    b.Property<int?>("RunwayLength")
                        .HasColumnType("integer")
                        .HasColumnName("length_ft");

                    b.Property<int?>("RunwayWidth")
                        .HasColumnType("integer")
                        .HasColumnName("width_ft");

                    b.Property<string>("Runway_Name")
                        .HasColumnType("text")
                        .HasColumnName("runway_name");

                    b.Property<string>("SurfaceText")
                        .HasColumnType("text")
                        .HasColumnName("surface");

                    b.Property<string>("Surface_Friendly")
                        .HasColumnType("text")
                        .HasColumnName("surface_friendly");

                    b.Property<int?>("fk_airport_id")
                        .HasColumnType("integer");

                    b.HasKey("Runway_Id")
                        .HasName("PK_Runway_Id");

                    b.HasIndex("Airport_Id")
                        .HasDatabaseName("IX_Runway_Airport_Id");

                    b.HasIndex("Runway_Name")
                        .HasDatabaseName("IX_Runway_Runway_Name");

                    b.ToTable("runways", t =>
                        {
                            t.Property("fk_airport_id")
                                .HasColumnName("fk_airport_id1");
                        });
                });

            modelBuilder.Entity("Airborn.web.Models.Airport", b =>
                {
                    b.HasOne("Airborn.web.Models.Country", "Country")
                        .WithMany("Airports")
                        .HasForeignKey("Country_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Airport_Country_Id");

                    b.HasOne("Airborn.web.Models.Region", "Region")
                        .WithMany("Airports")
                        .HasForeignKey("Region_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Airport_Region_Id");

                    b.Navigation("Country");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("Airborn.web.Models.Country", b =>
                {
                    b.HasOne("Airborn.web.Models.Continent", "Continent")
                        .WithMany("Countries")
                        .HasForeignKey("Continent_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Country_Continent_Id");

                    b.Navigation("Continent");
                });

            modelBuilder.Entity("Airborn.web.Models.Region", b =>
                {
                    b.HasOne("Airborn.web.Models.Country", "Country")
                        .WithMany("Regions")
                        .HasForeignKey("Country_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Region_Country_Id");

                    b.Navigation("Country");
                });

            modelBuilder.Entity("Airborn.web.Models.Runway", b =>
                {
                    b.HasOne("Airborn.web.Models.Airport", "Airport")
                        .WithMany("Runways")
                        .HasForeignKey("Airport_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Runway_Airport_Id");

                    b.Navigation("Airport");
                });

            modelBuilder.Entity("Airborn.web.Models.Airport", b =>
                {
                    b.Navigation("Runways");
                });

            modelBuilder.Entity("Airborn.web.Models.Continent", b =>
                {
                    b.Navigation("Countries");
                });

            modelBuilder.Entity("Airborn.web.Models.Country", b =>
                {
                    b.Navigation("Airports");

                    b.Navigation("Regions");
                });

            modelBuilder.Entity("Airborn.web.Models.Region", b =>
                {
                    b.Navigation("Airports");
                });
#pragma warning restore 612, 618
        }
    }
}