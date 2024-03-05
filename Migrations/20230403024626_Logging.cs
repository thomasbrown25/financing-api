using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace financing_api.Migrations
{
    public partial class Logging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                            name: "Logging",
                            columns: table => new
                            {
                                Id = table.Column<int>(type: "int", nullable: false)
                                    .Annotation("SqlServer:Identity", "1, 1"),
                                LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                                ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                                ExceptionStackTrace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                                ExceptionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                                CallingMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                                CallingAssemblyInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                            },
                            constraints: table =>
                            {
                                table.PrimaryKey("PK_Logging", x => x.Id);
                            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                            name: "Logging");
        }
    }
}
