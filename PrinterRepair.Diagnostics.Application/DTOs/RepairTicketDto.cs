namespace PrinterRepair.Diagnostics.Application.DTOs;

public sealed record RepairTicketDto(Guid Id, Guid ClientId, string PrinterModel, string Status, decimal? EstimatedCost);