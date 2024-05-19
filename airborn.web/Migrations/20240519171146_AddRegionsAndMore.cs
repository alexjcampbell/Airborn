using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class AddRegionsAndMore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "imported_runway_id",
                table: "runways",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "imported_country_id",
                table: "countries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "fk_country_id",
                table: "airports",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "imported_airport_id",
                table: "airports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "continents",
                columns: table => new
                {
                    continent_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContinentCode = table.Column<string>(name: "Continent Code", type: "text", nullable: true),
                    continent_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Continent_Id", x => x.continent_id);
                });

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    region_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    imported_region_id = table.Column<int>(type: "integer", nullable: false),
                    region_code = table.Column<string>(type: "text", nullable: true),
                    local_code = table.Column<string>(type: "text", nullable: true),
                    continent = table.Column<string>(type: "text", nullable: true),
                    iso_country = table.Column<string>(type: "text", nullable: true),
                    wikipedia_link = table.Column<string>(type: "text", nullable: true),
                    keywords = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region_Id", x => x.region_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_airports_fk_country_id",
                table: "airports",
                column: "fk_country_id");

            migrationBuilder.AddForeignKey(
                name: "FK_airports_countries_fk_country_id",
                table: "airports",
                column: "fk_country_id",
                principalTable: "countries",
                principalColumn: "country_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_airports_countries_fk_country_id",
                table: "airports");

            migrationBuilder.DropTable(
                name: "continents");

            migrationBuilder.DropTable(
                name: "regions");

            migrationBuilder.DropIndex(
                name: "IX_airports_fk_country_id",
                table: "airports");

            migrationBuilder.DropColumn(
                name: "imported_runway_id",
                table: "runways");

            migrationBuilder.DropColumn(
                name: "imported_country_id",
                table: "countries");

            migrationBuilder.DropColumn(
                name: "fk_country_id",
                table: "airports");

            migrationBuilder.DropColumn(
                name: "imported_airport_id",
                table: "airports");
        }
    }
}
