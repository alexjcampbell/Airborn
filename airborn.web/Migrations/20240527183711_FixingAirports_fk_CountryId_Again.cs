using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class FixingAirports_fk_CountryId_Again : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Country_Id",
                table: "airports",
                newName: "fk_country_id");

            migrationBuilder.RenameIndex(
                name: "IX_airports_Country_Id",
                table: "airports",
                newName: "IX_airports_fk_country_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "fk_country_id",
                table: "airports",
                newName: "Country_Id");

            migrationBuilder.RenameIndex(
                name: "IX_airports_fk_country_id",
                table: "airports",
                newName: "IX_airports_Country_Id");
        }
    }
}
