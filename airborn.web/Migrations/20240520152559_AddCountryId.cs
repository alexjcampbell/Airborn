using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_airports_regions_Region_Id1",
                table: "airports");

            migrationBuilder.DropIndex(
                name: "IX_airports_Region_Id1",
                table: "airports");

            migrationBuilder.DropColumn(
                name: "Region_Id1",
                table: "airports");

            migrationBuilder.RenameColumn(
                name: "Country_Id",
                table: "regions",
                newName: "fk_country_id");

            migrationBuilder.RenameIndex(
                name: "IX_regions_Country_Id",
                table: "regions",
                newName: "IX_regions_fk_country_id");

            migrationBuilder.AddColumn<int>(
                name: "fk_airport_id1",
                table: "runways",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "imported_airport_id",
                table: "runways",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "fk_country_id1",
                table: "regions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Airport_Region_Id",
                table: "airports",
                column: "Country_Id",
                principalTable: "regions",
                principalColumn: "region_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Airport_Region_Id",
                table: "airports");

            migrationBuilder.DropColumn(
                name: "fk_airport_id1",
                table: "runways");

            migrationBuilder.DropColumn(
                name: "imported_airport_id",
                table: "runways");

            migrationBuilder.DropColumn(
                name: "fk_country_id1",
                table: "regions");

            migrationBuilder.RenameColumn(
                name: "fk_country_id",
                table: "regions",
                newName: "Country_Id");

            migrationBuilder.RenameIndex(
                name: "IX_regions_fk_country_id",
                table: "regions",
                newName: "IX_regions_Country_Id");

            migrationBuilder.AddColumn<int>(
                name: "Region_Id1",
                table: "airports",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_airports_Region_Id1",
                table: "airports",
                column: "Region_Id1");

            migrationBuilder.AddForeignKey(
                name: "FK_airports_regions_Region_Id1",
                table: "airports",
                column: "Region_Id1",
                principalTable: "regions",
                principalColumn: "region_id");
        }
    }
}
