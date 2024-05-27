using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryAirportRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_airports_countries_fk_country_id",
                table: "airports");

            migrationBuilder.DropIndex(
                name: "IX_airports_fk_country_id",
                table: "airports");

            migrationBuilder.DropColumn(
                name: "fk_country_id",
                table: "airports");

            migrationBuilder.AddColumn<int>(
                name: "Country_Id",
                table: "airports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_airports_Country_Id",
                table: "airports",
                column: "Country_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Airport_Country_Id",
                table: "airports",
                column: "Country_Id",
                principalTable: "countries",
                principalColumn: "country_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Airport_Country_Id",
                table: "airports");

            migrationBuilder.DropIndex(
                name: "IX_airports_Country_Id",
                table: "airports");

            migrationBuilder.DropColumn(
                name: "Country_Id",
                table: "airports");

            migrationBuilder.AddColumn<int>(
                name: "fk_country_id",
                table: "airports",
                type: "integer",
                nullable: true);

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
    }
}
