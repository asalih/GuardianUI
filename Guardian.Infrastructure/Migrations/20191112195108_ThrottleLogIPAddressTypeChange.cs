using Microsoft.EntityFrameworkCore.Migrations;

namespace Guardian.Infrastructure.Migrations
{
    public partial class ThrottleLogIPAddressTypeChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                table: "ThrottleLogs",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "IPAddress",
                table: "ThrottleLogs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
