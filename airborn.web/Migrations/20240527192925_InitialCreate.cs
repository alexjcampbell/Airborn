using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "continents",
                columns: table => new
                {
                    continent_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    continent_code = table.Column<string>(type: "text", nullable: true),
                    last_updated_ts = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    continent_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Continent_Id", x => x.continent_id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    country_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    last_updated_ts = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    imported_country_id = table.Column<int>(type: "integer", nullable: false),
                    Continent_Id = table.Column<int>(type: "integer", nullable: false),
                    country_name = table.Column<string>(type: "text", nullable: true),
                    country_code = table.Column<string>(type: "text", nullable: true),
                    continent_code = table.Column<string>(type: "text", nullable: true),
                    wikipedia_link = table.Column<string>(type: "text", nullable: true),
                    keywords = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country_Id", x => x.country_id);
                    table.ForeignKey(
                        name: "FK_Country_Continent_Id",
                        column: x => x.Continent_Id,
                        principalTable: "continents",
                        principalColumn: "continent_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    region_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    imported_region_id = table.Column<int>(type: "integer", nullable: false),
                    fk_country_id = table.Column<int>(type: "integer", nullable: false),
                    last_updated_ts = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    region_code = table.Column<string>(type: "text", nullable: true),
                    local_code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    iso_country = table.Column<string>(type: "text", nullable: true),
                    wikipedia_link = table.Column<string>(type: "text", nullable: true),
                    keywords = table.Column<string>(type: "text", nullable: true),
                    fk_country_id1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region_Id", x => x.region_id);
                    table.ForeignKey(
                        name: "FK_Region_Country_Id",
                        column: x => x.fk_country_id,
                        principalTable: "countries",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "airports",
                columns: table => new
                {
                    airport_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    last_updated_ts = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    imported_airport_id = table.Column<int>(type: "integer", nullable: false),
                    ident = table.Column<string>(type: "text", nullable: true),
                    elevation_ft = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    latitude_deg = table.Column<double>(type: "double precision", nullable: true),
                    longitude_deg = table.Column<double>(type: "double precision", nullable: true),
                    magnetic_variation = table.Column<double>(type: "double precision", nullable: true),
                    municipality = table.Column<string>(type: "text", nullable: true),
                    iso_region = table.Column<string>(type: "text", nullable: true),
                    fk_region_id = table.Column<int>(type: "integer", nullable: false),
                    iso_country = table.Column<string>(type: "text", nullable: true),
                    fk_country_id = table.Column<int>(type: "integer", nullable: false),
                    scheduled_service = table.Column<string>(type: "text", nullable: true),
                    gps_code = table.Column<string>(type: "text", nullable: true),
                    iata_code = table.Column<string>(type: "text", nullable: true),
                    local_code = table.Column<string>(type: "text", nullable: true),
                    home_link = table.Column<string>(type: "text", nullable: true),
                    wikipedia_link = table.Column<string>(type: "text", nullable: true),
                    keywords = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airport_Id", x => x.airport_id);
                    table.ForeignKey(
                        name: "FK_Airport_Country_Id",
                        column: x => x.fk_country_id,
                        principalTable: "countries",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Airport_Region_Id",
                        column: x => x.fk_region_id,
                        principalTable: "regions",
                        principalColumn: "region_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "runways",
                columns: table => new
                {
                    runway_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_airport_id = table.Column<int>(type: "integer", nullable: false),
                    imported_airport_id = table.Column<int>(type: "integer", nullable: false),
                    last_updated_ts = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    imported_runway_id = table.Column<int>(type: "integer", nullable: false),
                    airport_ident = table.Column<string>(type: "text", nullable: true),
                    runway_name = table.Column<string>(type: "text", nullable: true),
                    length_ft = table.Column<int>(type: "integer", nullable: true),
                    width_ft = table.Column<int>(type: "integer", nullable: true),
                    lighted = table.Column<string>(type: "text", nullable: true),
                    closed = table.Column<string>(type: "text", nullable: true),
                    latitude_deg = table.Column<double>(type: "double precision", nullable: true),
                    longitude_deg = table.Column<double>(type: "double precision", nullable: true),
                    surface = table.Column<string>(type: "text", nullable: true),
                    surface_friendly = table.Column<string>(type: "text", nullable: true),
                    elevation_ft = table.Column<int>(type: "integer", nullable: true),
                    displaced_threshold_ft = table.Column<int>(type: "integer", nullable: true),
                    heading_degt = table.Column<double>(type: "double precision", nullable: true),
                    fk_airport_id1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runway_Id", x => x.runway_id);
                    table.ForeignKey(
                        name: "FK_Runway_Airport_Id",
                        column: x => x.fk_airport_id,
                        principalTable: "airports",
                        principalColumn: "airport_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Airport_Ident",
                table: "airports",
                column: "ident",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Airport_Type",
                table: "airports",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_airports_fk_country_id",
                table: "airports",
                column: "fk_country_id");

            migrationBuilder.CreateIndex(
                name: "IX_airports_fk_region_id",
                table: "airports",
                column: "fk_region_id");

            migrationBuilder.CreateIndex(
                name: "IX_countries_Continent_Id",
                table: "countries",
                column: "Continent_Id");

            migrationBuilder.CreateIndex(
                name: "IX_regions_fk_country_id",
                table: "regions",
                column: "fk_country_id");

            migrationBuilder.CreateIndex(
                name: "IX_Runway_Airport_Id",
                table: "runways",
                column: "fk_airport_id");

            migrationBuilder.CreateIndex(
                name: "IX_Runway_Runway_Name",
                table: "runways",
                column: "runway_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "runways");

            migrationBuilder.DropTable(
                name: "airports");

            migrationBuilder.DropTable(
                name: "regions");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropTable(
                name: "continents");
        }
    }
}
