using Microsoft.EntityFrameworkCore.Migrations;

namespace Guardian.Infrastructure.Migrations
{
    public partial class RuleForAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RuleFor",
                table: "RuleLogs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RuleFor",
                table: "FirewallRules",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RuleFor",
                table: "RuleLogs");

            migrationBuilder.DropColumn(
                name: "RuleFor",
                table: "FirewallRules");
        }
    }
}
