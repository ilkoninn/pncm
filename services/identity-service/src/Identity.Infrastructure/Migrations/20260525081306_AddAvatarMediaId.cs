using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarMediaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vendor_profiles");

            migrationBuilder.DropColumn(
                name: "avatar_url",
                table: "users");

            migrationBuilder.AddColumn<Guid>(
                name: "avatar_media_id",
                table: "users",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar_media_id",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "avatar_url",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "vendor_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    logo_url = table.Column<string>(type: "text", nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    store_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendor_profiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_vendor_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vendor_profiles_latitude_longitude",
                table: "vendor_profiles",
                columns: new[] { "latitude", "longitude" });

            migrationBuilder.CreateIndex(
                name: "ix_vendor_profiles_store_name",
                table: "vendor_profiles",
                column: "store_name");

            migrationBuilder.CreateIndex(
                name: "ix_vendor_profiles_user_id",
                table: "vendor_profiles",
                column: "user_id",
                unique: true);
        }
    }
}
