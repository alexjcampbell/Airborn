using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class AddAirportsRegionsRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "continent_code",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "continent",
                table: "airports");

            migrationBuilder.AddColumn<int>(
                name: "Region_Id1",
                table: "airports",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fk_region_id",
                table: "airports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "fk_region_id",
                table: "airports");

            migrationBuilder.AddColumn<string>(
                name: "continent_code",
                table: "regions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "continent",
                table: "airports",
                type: "text",
                nullable: true);
        }
    }
}
