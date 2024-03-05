using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace financing_api.Migrations
{
    public partial class ReccuringCategoryUpdateDueDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Due",
                table: "Recurrings");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Recurrings",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Recurrings");

            migrationBuilder.AddColumn<string>(
                name: "Due",
                table: "Recurrings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
