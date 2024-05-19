using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class AddRegions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "country_namet",
                table: "countries",
                newName: "country_name");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "countries",
                newName: "country_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "country_name",
                table: "countries",
                newName: "country_namet");

            migrationBuilder.RenameColumn(
                name: "country_id",
                table: "countries",
                newName: "country");
        }
    }
}
