using PrinterRepair.Diagnostics.Application.DTOs;

namespace PrinterRepair.Diagnostics.Application.Services;

public interface IRepairTicketService
{
    Task<RepairTicketDto> SubmitRequestAsync(Guid clientId, string printerModel, CancellationToken cancellationToken = default);
    Task<RepairTicketDto?> GetTicketAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RepairTicketDto> IssueEstimateAsync(Guid id, decimal estimatedCost, CancellationToken cancellationToken = default);
    Task<RepairTicketDto> ApproveEstimateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RepairTicketDto> CompleteRepairAsync(Guid id, bool passedCalibrationTest, CancellationToken cancellationToken = default);
}