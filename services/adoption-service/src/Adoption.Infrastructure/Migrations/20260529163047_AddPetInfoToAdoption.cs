using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adoption.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPetInfoToAdoption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pet_name",
                table: "adoption_requests",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "pet_primary_photo_url",
                table: "adoption_requests",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pet_slug",
                table: "adoption_requests",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pet_name",
                table: "adoption_requests");

            migrationBuilder.DropColumn(
                name: "pet_primary_photo_url",
                table: "adoption_requests");

            migrationBuilder.DropColumn(
                name: "pet_slug",
                table: "adoption_requests");
        }
    }
}
