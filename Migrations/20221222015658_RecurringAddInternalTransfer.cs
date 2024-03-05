using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace financing_api.Migrations
{
    public partial class RecurringAddInternalTransfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InternalTransfer",
                table: "Recurrings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalTransfer",
                table: "Recurrings");
        }
    }
}
