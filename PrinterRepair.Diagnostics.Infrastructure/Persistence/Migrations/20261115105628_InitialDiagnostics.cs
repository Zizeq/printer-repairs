using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrinterRepair.Diagnostics.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDiagnostics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "diagnostics");

            migrationBuilder.CreateTable(
                name: "RepairTickets",
                schema: "diagnostics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrinterModel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RepairCostEstimate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiagnosticNotes",
                schema: "diagnostics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosticNotes", x => new { x.RepairTicketId, x.Id });
                    table.ForeignKey(
                        name: "FK_DiagnosticNotes_RepairTickets_RepairTicketId",
                        column: x => x.RepairTicketId,
                        principalSchema: "diagnostics",
                        principalTable: "RepairTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiagnosticNotes",
                schema: "diagnostics");

            migrationBuilder.DropTable(
                name: "RepairTickets",
                schema: "diagnostics");
        }
    }
}
