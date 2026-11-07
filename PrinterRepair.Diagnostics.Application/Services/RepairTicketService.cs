using PrinterRepair.Diagnostics.Application.DTOs;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Domain.Aggregates;

namespace PrinterRepair.Diagnostics.Application.Services;

public sealed class RepairTicketService : IRepairTicketService
{
    private readonly IRepairTicketRepository _repository;

    public RepairTicketService(IRepairTicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<RepairTicketDto> SubmitRequestAsync(Guid clientId, string printerModel, CancellationToken cancellationToken = default)
    {
        var ticket = new RepairTicket(clientId, printerModel);
        await _repository.SaveAsync(ticket, cancellationToken);
        return MapToDto(ticket);
    }

    public async Task<RepairTicketDto?> GetTicketAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await _repository.GetByIdAsync(id, cancellationToken);
        return ticket is null ? null : MapToDto(ticket);
    }

    public async Task<RepairTicketDto> IssueEstimateAsync(Guid id, decimal estimatedCost, CancellationToken cancellationToken = default)
    {
        var ticket = await _repository.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
            throw new Exception("Ticket not found.");

        ticket.IssueEstimate(estimatedCost);
        await _repository.SaveAsync(ticket, cancellationToken);

        return MapToDto(ticket);
    }

    private static RepairTicketDto MapToDto(RepairTicket ticket) =>
        new(ticket.Id, ticket.ClientId, ticket.PrinterModel, ticket.Status.ToString(), ticket.RepairCostEstimate);
}