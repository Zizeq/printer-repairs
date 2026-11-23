using Microsoft.EntityFrameworkCore;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Domain.Aggregates;
using PrinterRepair.Diagnostics.Infrastructure.Persistence;

namespace PrinterRepair.Diagnostics.Infrastructure.Repositories;

internal sealed class RepairTicketRepository : IRepairTicketRepository
{
    private readonly DiagnosticsDbContext _dbContext;

    public RepairTicketRepository(DiagnosticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepairTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RepairTickets
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<RepairTicket?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RepairTickets
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<RepairTicket>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.RepairTickets
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task SaveAsync(RepairTicket ticket, CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.RepairTickets.AnyAsync(t => t.Id == ticket.Id, cancellationToken);
        if (!exists)
        {
            _dbContext.RepairTickets.Add(ticket);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(RepairTicket ticket, CancellationToken cancellationToken = default)
    {
        _dbContext.RepairTickets.Remove(ticket);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}