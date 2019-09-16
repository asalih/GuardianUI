using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Guardian.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
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
                    table.PrimaryKey("PK_Account", x => x.Id);
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
                    UseHttps = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Targets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Targets_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "Targets");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
