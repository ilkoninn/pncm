using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "pets",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE pets
                SET slug = LOWER(REGEXP_REPLACE(name, '[^a-zA-Z0-9]+', '-', 'g'))
                        || '-' || RIGHT(REPLACE(id::text, '-', ''), 12)
                WHERE slug = ''
                """);

            migrationBuilder.CreateIndex(
                name: "ix_pets_slug",
                table: "pets",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_pets_slug",
                table: "pets");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "pets");
        }
    }
}
