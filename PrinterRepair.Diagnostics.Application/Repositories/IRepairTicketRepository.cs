using PrinterRepair.Diagnostics.Domain.Aggregates;

namespace PrinterRepair.Diagnostics.Application.Repositories;

public interface IRepairTicketRepository
{
    Task<RepairTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RepairTicket?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RepairTicket>> GetAllAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(RepairTicket ticket, CancellationToken cancellationToken = default);
    Task DeleteAsync(RepairTicket ticket, CancellationToken cancellationToken = default);
}