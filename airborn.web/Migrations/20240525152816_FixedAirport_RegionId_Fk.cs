using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class FixedAirport_RegionId_Fk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Airport_Region_Id",
                table: "airports");

            migrationBuilder.CreateIndex(
                name: "IX_airports_fk_region_id",
                table: "airports",
                column: "fk_region_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Airport_Region_Id",
                table: "airports",
                column: "fk_region_id",
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

            migrationBuilder.DropIndex(
                name: "IX_airports_fk_region_id",
                table: "airports");

            migrationBuilder.AddForeignKey(
                name: "FK_Airport_Region_Id",
                table: "airports",
                column: "Country_Id",
                principalTable: "regions",
                principalColumn: "region_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
