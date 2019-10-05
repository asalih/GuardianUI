using Microsoft.EntityFrameworkCore.Migrations;

namespace Guardian.Infrastructure.Migrations
{
    public partial class ResponseExecutionElapsedAddedHttpLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RuleCheckElapsed",
                table: "HTTPLogs",
                newName: "ResponseRulesCheckElapsed");

            migrationBuilder.AddColumn<long>(
                name: "RequestRulesCheckElapsed",
                table: "HTTPLogs",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestRulesCheckElapsed",
                table: "HTTPLogs");

            migrationBuilder.RenameColumn(
                name: "ResponseRulesCheckElapsed",
                table: "HTTPLogs",
                newName: "RuleCheckElapsed");
        }
    }
}
