using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class AddSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "regions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "countries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "continents",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_regions_slug",
                table: "regions",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_countries_slug",
                table: "countries",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_continents_slug",
                table: "continents",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_regions_slug",
                table: "regions");

            migrationBuilder.DropIndex(
                name: "IX_countries_slug",
                table: "countries");

            migrationBuilder.DropIndex(
                name: "IX_continents_slug",
                table: "continents");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "countries");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "continents");
        }
    }
}
