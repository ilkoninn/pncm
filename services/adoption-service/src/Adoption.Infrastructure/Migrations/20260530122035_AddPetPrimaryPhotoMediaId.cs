using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adoption.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPetPrimaryPhotoMediaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "pet_primary_photo_media_id",
                table: "adoption_requests",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pet_primary_photo_media_id",
                table: "adoption_requests");
        }
    }
}
