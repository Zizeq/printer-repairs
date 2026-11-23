using PrinterRepair.Diagnostics.Application.Abstractions.Messaging;
using PrinterRepair.Diagnostics.Application.DTOs;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Domain.Exceptions;

namespace PrinterRepair.Diagnostics.Application.Features.RepairTickets.Commands;

public sealed record CompleteRepairCommand(Guid TicketId, bool PassedCalibrationTest) : ICommand<RepairTicketDto>;

public sealed class CompleteRepairCommandHandler : ICommandHandler<CompleteRepairCommand, RepairTicketDto>
{
    private readonly IRepairTicketRepository _repository;

    public CompleteRepairCommandHandler(IRepairTicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<RepairTicketDto> HandleAsync(
        CompleteRepairCommand command,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _repository.GetByIdAsync(command.TicketId, cancellationToken)
            ?? throw new RepairTicketNotFoundException(command.TicketId);

        ticket.CompleteRepair(command.PassedCalibrationTest);
        await _repository.SaveAsync(ticket, cancellationToken);

        return RepairTicketDto.FromDomain(ticket);
    }
}