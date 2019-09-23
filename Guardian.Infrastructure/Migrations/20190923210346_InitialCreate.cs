using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Guardian.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    Email = table.Column<string>(maxLength: 200, nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Salt = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Targets",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    Domain = table.Column<string>(maxLength: 250, nullable: true),
                    OriginIpAddress = table.Column<string>(maxLength: 250, nullable: true),
                    CertKey = table.Column<string>(nullable: true),
                    CertCrt = table.Column<string>(nullable: true),
                    AccountId = table.Column<Guid>(nullable: false),
                    UseHttps = table.Column<bool>(nullable: false),
                    WAFEnabled = table.Column<bool>(nullable: false),
                    IsVerified = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Targets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Targets_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FirewallRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    TargetId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Expression = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirewallRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FirewallRules_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FirewallRules_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HTTPLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    TargetId = table.Column<Guid>(nullable: false),
                    RequestUri = table.Column<string>(nullable: true),
                    StatusCode = table.Column<int>(nullable: false),
                    RuleCheckElapsed = table.Column<long>(nullable: false),
                    HttpElapsed = table.Column<long>(nullable: false),
                    RequestSize = table.Column<long>(nullable: false),
                    ResponseSize = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HTTPLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HTTPLogs_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    TargetId = table.Column<Guid>(nullable: false),
                    IsHitted = table.Column<bool>(nullable: false),
                    ExecutionMillisecond = table.Column<int>(nullable: false),
                    LogType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FirewallRuleId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RuleLogs_FirewallRules_FirewallRuleId",
                        column: x => x.FirewallRuleId,
                        principalTable: "FirewallRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RuleLogs_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FirewallRules_AccountId",
                table: "FirewallRules",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FirewallRules_TargetId",
                table: "FirewallRules",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_HTTPLogs_TargetId",
                table: "HTTPLogs",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleLogs_FirewallRuleId",
                table: "RuleLogs",
                column: "FirewallRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleLogs_TargetId",
                table: "RuleLogs",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Targets_AccountId",
                table: "Targets",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Targets_Domain",
                table: "Targets",
                column: "Domain",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HTTPLogs");

            migrationBuilder.DropTable(
                name: "RuleLogs");

            migrationBuilder.DropTable(
                name: "FirewallRules");

            migrationBuilder.DropTable(
                name: "Targets");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
