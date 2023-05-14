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
                name: "airports",
                columns: table => new
                {
                    airport_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ident = table.Column<string>(type: "text", nullable: true),
                    elevation_ft = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    latitude_deg = table.Column<double>(type: "double precision", nullable: true),
                    longitude_deg = table.Column<double>(type: "double precision", nullable: true),
                    magnetic_variation = table.Column<double>(type: "double precision", nullable: true),
                    municipality = table.Column<string>(type: "text", nullable: true),
                    iso_region = table.Column<string>(type: "text", nullable: true),
                    continent = table.Column<string>(type: "text", nullable: true),
                    iso_country = table.Column<string>(type: "text", nullable: true),
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
                });

            migrationBuilder.CreateTable(
                name: "runways",
                columns: table => new
                {
                    runway_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_airport_id = table.Column<int>(type: "integer", nullable: false),
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
                    heading_degt = table.Column<double>(type: "double precision", nullable: true)
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
        }
    }
}
