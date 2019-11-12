using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Guardian.Infrastructure.Migrations
{
    public partial class ThrottleLogsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThrottleLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IPAddress = table.Column<long>(nullable: false),
                    ThrottleType = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThrottleLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThrottleLogs");
        }
    }
}
