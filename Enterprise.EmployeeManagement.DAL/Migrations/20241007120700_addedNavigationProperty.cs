using Microsoft.EntityFrameworkCore.Migrations;

namespace Enterprise.EmployeeManagement.DAL.Migrations
{
    public partial class addedNavigationProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiteUsers_Roles_RoleId",
                table: "SiteUsers");

            migrationBuilder.DropIndex(
                name: "IX_SiteUsers_RoleId",
                table: "SiteUsers");
        }
    }
}
