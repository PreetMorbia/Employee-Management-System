using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Enterprise.EmployeeManagement.DAL.Migrations
{
    public partial class AddedTaskModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SiteUsers_EmailAddress",
                table: "SiteUsers");

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    TaskId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TaskName = table.Column<string>(maxLength: 100, nullable: false),
                    TaskDescription = table.Column<string>(nullable: false),
                    AssignedBy = table.Column<int>(nullable: false),
                    AssignedTo = table.Column<int>(nullable: false),
                    AssignedDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Deadline = table.Column<DateTime>(nullable: false),
                    TaskStatus = table.Column<string>(nullable: false),
                    priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.TaskId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_SiteUsers_EmailAddress",
                table: "SiteUsers",
                column: "EmailAddress",
                unique: true);
        }
    }
}
