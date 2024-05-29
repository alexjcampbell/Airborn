using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class MakeRegionSlugNotUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_regions_slug",
                table: "regions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_regions_slug",
                table: "regions",
                column: "slug",
                unique: true);
        }
    }
}
