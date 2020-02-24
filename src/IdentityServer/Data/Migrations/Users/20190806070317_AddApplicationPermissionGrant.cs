using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Migrations
{
    public partial class AddApplicationPermissionGrant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PermissionGrants",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    TenantId = table.Column<string>(maxLength: 128, nullable: true),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: true),
                    ProviderType = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionGrants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPermissionGrant_Name_ProviderType_ProviderKey",
                table: "PermissionGrants",
                columns: new[] { "Name", "ProviderType", "ProviderKey" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionGrants");
        }
    }
}
