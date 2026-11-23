using PrinterRepair.Diagnostics.Domain.Aggregates;

namespace PrinterRepair.Diagnostics.Application.DTOs;

public sealed record RepairTicketDto(
    Guid Id,
    Guid ClientId,
    string PrinterModel,
    string Status,
    decimal? EstimatedCost)
{
    public static RepairTicketDto FromDomain(RepairTicket ticket) =>
        new(
            ticket.Id,
            ticket.ClientId,
            ticket.PrinterModel,
            ticket.Status.ToString(),
            ticket.RepairCostEstimate);
}