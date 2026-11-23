using PrinterRepair.Diagnostics.Application.Abstractions.Messaging;
using PrinterRepair.Diagnostics.Application.DTOs;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Domain.Exceptions;

namespace PrinterRepair.Diagnostics.Application.Features.RepairTickets.Commands;

public sealed record IssueEstimateCommand(Guid TicketId, decimal EstimatedCost) : ICommand<RepairTicketDto>;

public sealed class IssueEstimateCommandHandler : ICommandHandler<IssueEstimateCommand, RepairTicketDto>
{
    private readonly IRepairTicketRepository _repository;

    public IssueEstimateCommandHandler(IRepairTicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<RepairTicketDto> HandleAsync(
        IssueEstimateCommand command,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _repository.GetByIdAsync(command.TicketId, cancellationToken)
            ?? throw new RepairTicketNotFoundException(command.TicketId);

        ticket.IssueEstimate(command.EstimatedCost);
        await _repository.SaveAsync(ticket, cancellationToken);

        return RepairTicketDto.FromDomain(ticket);
    }
}