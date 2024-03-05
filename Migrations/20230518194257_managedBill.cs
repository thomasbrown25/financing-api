using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace financing_api.Migrations
{
    public partial class managedBill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                            name: "ManagedBills",
                            columns: table => new
                            {
                                Id = table.Column<int>(type: "int", nullable: false)
                                    .Annotation("SqlServer:Identity", "1, 1"),
                                UserId = table.Column<int>(type: "int", nullable: false),
                                Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                                Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                                MerchantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                                FirstDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                                LastDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                                Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                                IsActive = table.Column<bool>(type: "bit", nullable: false),
                            },
                            constraints: table =>
                            {
                                table.PrimaryKey("PK_ManagedBills", x => x.Id);
                            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
