using System.Collections.Concurrent;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Domain.Aggregates;

namespace PrinterRepair.Diagnostics.Presentation.Infrastructure;

public sealed class InMemoryRepairTicketRepository : IRepairTicketRepository
{
    private readonly ConcurrentDictionary<Guid, RepairTicket> _storage = new();

    public Task<RepairTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _storage.TryGetValue(id, out var ticket);
        return Task.FromResult(ticket);
    }

    public Task SaveAsync(RepairTicket ticket, CancellationToken cancellationToken = default)
    {
        _storage[ticket.Id] = ticket;
        return Task.CompletedTask;
    }
}