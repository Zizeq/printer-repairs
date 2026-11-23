using System.Collections.Concurrent;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Domain.Aggregates;

namespace PrinterRepair.Diagnostics.Presentation.Infrastructure;

internal sealed class InMemoryRepairTicketRepository : IRepairTicketRepository
{
    private readonly ConcurrentDictionary<Guid, RepairTicket> _tickets = new();

    public Task<RepairTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_tickets.GetValueOrDefault(id));

    public Task<RepairTicket?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_tickets.GetValueOrDefault(id));

    public Task<IReadOnlyList<RepairTicket>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<RepairTicket>>(_tickets.Values.ToList());

    public Task SaveAsync(RepairTicket ticket, CancellationToken cancellationToken = default)
    {
        _tickets[ticket.Id] = ticket;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(RepairTicket ticket, CancellationToken cancellationToken = default)
    {
        _tickets.TryRemove(ticket.Id, out _);
        return Task.CompletedTask;
    }
}