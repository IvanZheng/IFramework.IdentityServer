using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Migrations
{
    public partial class AddMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TenantId = table.Column<string>(nullable: true),
                    DefaultNodeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NodeMembers",
                columns: table => new
                {
                    MemberId = table.Column<string>(nullable: false),
                    NodeId = table.Column<string>(nullable: false),
                    TenantId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeMembers", x => new { x.MemberId, x.NodeId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_UserId_TenantId",
                table: "Members",
                columns: new[] { "UserId", "TenantId" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [TenantId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "NodeMembers");
        }
    }
}
