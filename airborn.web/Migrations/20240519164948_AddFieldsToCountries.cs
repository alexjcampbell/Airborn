using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace airborn.web.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "continent",
                table: "countries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "keywords",
                table: "countries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "wikipedia_link",
                table: "countries",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "continent",
                table: "countries");

            migrationBuilder.DropColumn(
                name: "keywords",
                table: "countries");

            migrationBuilder.DropColumn(
                name: "wikipedia_link",
                table: "countries");
        }
    }
}
