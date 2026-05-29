using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerNameToPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "owner_first_name",
                table: "pets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "owner_last_name",
                table: "pets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "owner_first_name",
                table: "pets");

            migrationBuilder.DropColumn(
                name: "owner_last_name",
                table: "pets");
        }
    }
}
