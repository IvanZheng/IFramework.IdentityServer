using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Migrations
{
    public partial class ReIndexForPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationPermissionGrant_Name_ProviderType_ProviderKey",
                table: "PermissionGrants");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                name: "ScopeId",
                table: "PermissionGrants",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPermissionGrant_Name_ProviderType_ProviderKey",
                table: "PermissionGrants",
                columns: new[] { "Name", "ProviderType", "ProviderKey", "ScopeId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                columns: new[] { "NormalizedName", "ScopeId" },
                unique: true,
                filter: "([NormalizedName] IS NOT NULL)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationPermissionGrant_Name_ProviderType_ProviderKey",
                table: "PermissionGrants");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "ScopeId",
                table: "PermissionGrants");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPermissionGrant_Name_ProviderType_ProviderKey",
                table: "PermissionGrants",
                columns: new[] { "Name", "ProviderType", "ProviderKey" });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                columns: new[] { "NormalizedName", "ScopeId" },
                unique: true,
                filter: "[NormalizedName] IS NOT NULL AND [ScopeId] IS NOT NULL");
        }
    }
}
