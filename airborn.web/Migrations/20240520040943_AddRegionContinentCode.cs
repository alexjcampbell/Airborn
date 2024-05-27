using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class AddRegionContinentCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "continent",
                table: "regions",
                newName: "name");

            migrationBuilder.AddColumn<int>(
                name: "Country_Id",
                table: "regions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "continent_code",
                table: "regions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_regions_Country_Id",
                table: "regions",
                column: "Country_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Region_Country_Id",
                table: "regions",
                column: "Country_Id",
                principalTable: "countries",
                principalColumn: "country_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Region_Country_Id",
                table: "regions");

            migrationBuilder.DropIndex(
                name: "IX_regions_Country_Id",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "Country_Id",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "continent_code",
                table: "regions");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "regions",
                newName: "continent");
        }
    }
}
