using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesToCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_countries_continents_Continent_Id",
                table: "countries");

            migrationBuilder.AlterColumn<int>(
                name: "Continent_Id",
                table: "countries",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Country_Continent_Id",
                table: "countries",
                column: "Continent_Id",
                principalTable: "continents",
                principalColumn: "continent_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Country_Continent_Id",
                table: "countries");

            migrationBuilder.AlterColumn<int>(
                name: "Continent_Id",
                table: "countries",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_countries_continents_Continent_Id",
                table: "countries",
                column: "Continent_Id",
                principalTable: "continents",
                principalColumn: "continent_id");
        }
    }
}
