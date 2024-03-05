using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace financing_api.Migrations
{
    public partial class LoggingDataExchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
               name: "LoggingDataExchange",
               columns: table => new
               {
                   Id = table.Column<int>(type: "int", nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                   MessageSource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   MessageTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   MessagePayload = table.Column<string>(type: "nvarchar(max)", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_LoggingDataExchange", x => x.Id);
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoggingDataExchange");

        }
    }
}
