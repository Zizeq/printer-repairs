using Microsoft.EntityFrameworkCore;
using PrinterRepair.Diagnostics.Domain.Aggregates;

namespace PrinterRepair.Diagnostics.Infrastructure.Persistence;

internal sealed class DiagnosticsDbContext : DbContext
{
    public DiagnosticsDbContext(DbContextOptions<DiagnosticsDbContext> options)
        : base(options)
    {
    }

    public DbSet<RepairTicket> RepairTickets => Set<RepairTicket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiagnosticsDbContext).Assembly);
    }
}