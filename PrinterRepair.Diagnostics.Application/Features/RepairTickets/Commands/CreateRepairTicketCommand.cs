using PrinterRepair.Diagnostics.Application.Abstractions.Messaging;
using PrinterRepair.Diagnostics.Application.DTOs;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Domain.Aggregates;

namespace PrinterRepair.Diagnostics.Application.Features.RepairTickets.Commands;

public sealed record CreateRepairTicketCommand(Guid ClientId, string PrinterModel) : ICommand<RepairTicketDto>;

internal sealed class CreateRepairTicketCommandHandler : ICommandHandler<CreateRepairTicketCommand, RepairTicketDto>
{
    private readonly IRepairTicketRepository _repository;

    public CreateRepairTicketCommandHandler(IRepairTicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<RepairTicketDto> HandleAsync(
        CreateRepairTicketCommand command,
        CancellationToken cancellationToken = default)
    {
        var ticket = new RepairTicket(command.ClientId, command.PrinterModel);
        await _repository.SaveAsync(ticket, cancellationToken);
        return RepairTicketDto.FromDomain(ticket);
    }
}