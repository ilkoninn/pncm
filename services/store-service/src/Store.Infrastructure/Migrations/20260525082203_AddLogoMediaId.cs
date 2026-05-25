using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLogoMediaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "logo_url",
                table: "stores");

            migrationBuilder.AddColumn<Guid>(
                name: "logo_media_id",
                table: "stores",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "logo_media_id",
                table: "stores");

            migrationBuilder.AddColumn<string>(
                name: "logo_url",
                table: "stores",
                type: "text",
                nullable: true);
        }
    }
}
