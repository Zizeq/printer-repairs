using PrinterRepair.Diagnostics.Application.Abstractions.Messaging;
using PrinterRepair.Diagnostics.Application.DTOs;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Domain.Exceptions;

namespace PrinterRepair.Diagnostics.Application.Features.RepairTickets.Commands;

public sealed record ApproveEstimateCommand(Guid TicketId) : ICommand<RepairTicketDto>;

public sealed class ApproveEstimateCommandHandler : ICommandHandler<ApproveEstimateCommand, RepairTicketDto>
{
    private readonly IRepairTicketRepository _repository;

    public ApproveEstimateCommandHandler(IRepairTicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<RepairTicketDto> HandleAsync(
        ApproveEstimateCommand command,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _repository.GetByIdAsync(command.TicketId, cancellationToken)
            ?? throw new RepairTicketNotFoundException(command.TicketId);

        ticket.ApproveEstimate();
        await _repository.SaveAsync(ticket, cancellationToken);

        return RepairTicketDto.FromDomain(ticket);
    }
}