using PrinterRepair.Diagnostics.Domain.Aggregates;

namespace PrinterRepair.Diagnostics.Application.Repositories;

public interface IRepairTicketRepository
{
    Task<RepairTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveAsync(RepairTicket ticket, CancellationToken cancellationToken = default);
}