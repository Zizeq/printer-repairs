using Microsoft.EntityFrameworkCore;
using PrinterRepair.Diagnostics.Domain.Aggregates;

namespace PrinterRepair.Diagnostics.Infrastructure.Persistence;

internal static class DiagnosticsDataSeeder
{
    public static async Task SeedAsync(DiagnosticsDbContext context)
    {
        if (context.Database.IsSqlServer())
            await context.Database.MigrateAsync();
        else
            await context.Database.EnsureCreatedAsync();

        if (await context.RepairTickets.AnyAsync())
            return;

        var ticket1 = new RepairTicket(Guid.NewGuid(), "HP LaserJet Pro M404n");
        ticket1.AddDiagnosticNote("Customer reports paper jam in tray 2.");

        var ticket2 = new RepairTicket(Guid.NewGuid(), "Epson EcoTank ET-4760");
        ticket2.AddDiagnosticNote("Streaky printing on all colors.");
        ticket2.IssueEstimate(150.00m);

        context.RepairTickets.AddRange(ticket1, ticket2);
        await context.SaveChangesAsync();
    }
}