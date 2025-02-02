using Microsoft.EntityFrameworkCore.Migrations;

namespace Enterprise.EmployeeManagement.DAL.Migrations
{
    public partial class RemovedNavigationProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiteUsers_Roles_RoleId",
                table: "SiteUsers");

            migrationBuilder.DropIndex(
                name: "IX_SiteUsers_RoleId",
                table: "SiteUsers");

            migrationBuilder.CreateIndex(
                name: "IX_SiteUsers_EmailAddress",
                table: "SiteUsers",
                column: "EmailAddress",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SiteUsers_EmailAddress",
                table: "SiteUsers");

            migrationBuilder.CreateIndex(
                name: "IX_SiteUsers_RoleId",
                table: "SiteUsers",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_SiteUsers_Roles_RoleId",
                table: "SiteUsers",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
