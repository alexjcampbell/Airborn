using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "airports",
                columns: table => new
                {
                    Airport_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ident = table.Column<string>(type: "text", nullable: true),
                    Elevation_Ft = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Latitude_Deg = table.Column<double>(type: "double precision", nullable: true),
                    Longitude_Deg = table.Column<double>(type: "double precision", nullable: true),
                    Magnetic_Variation = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Airport_Id", x => x.Airport_Id);
                });

            migrationBuilder.CreateTable(
                name: "Runways",
                columns: table => new
                {
                    Runway_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_Airport_Id = table.Column<int>(type: "integer", nullable: false),
                    Airport_Ident = table.Column<string>(type: "text", nullable: true),
                    Runway_Name = table.Column<string>(type: "text", nullable: true),
                    Length_Ft = table.Column<int>(type: "integer", nullable: true),
                    Width_Ft = table.Column<int>(type: "integer", nullable: true),
                    Surface = table.Column<string>(type: "text", nullable: true),
                    Elevation_Ft = table.Column<int>(type: "integer", nullable: true),
                    Displaced_Threshold_Ft = table.Column<int>(type: "integer", nullable: true),
                    heading_degT = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Runway_Id", x => x.Runway_Id);
                    table.ForeignKey(
                        name: "FK_Runways_airports_fk_Airport_Id",
                        column: x => x.fk_Airport_Id,
                        principalTable: "airports",
                        principalColumn: "Airport_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Runways_fk_Airport_Id",
                table: "Runways",
                column: "fk_Airport_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Runways");

            migrationBuilder.DropTable(
                name: "airports");
        }
    }
}
