using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesToContinents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Continent Code",
                table: "continents",
                newName: "continent_code");

            migrationBuilder.AddColumn<int>(
                name: "Continent_Id",
                table: "countries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_countries_Continent_Id",
                table: "countries",
                column: "Continent_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_countries_continents_Continent_Id",
                table: "countries",
                column: "Continent_Id",
                principalTable: "continents",
                principalColumn: "continent_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_countries_continents_Continent_Id",
                table: "countries");

            migrationBuilder.DropIndex(
                name: "IX_countries_Continent_Id",
                table: "countries");

            migrationBuilder.DropColumn(
                name: "Continent_Id",
                table: "countries");

            migrationBuilder.RenameColumn(
                name: "continent_code",
                table: "continents",
                newName: "Continent Code");
        }
    }
}
