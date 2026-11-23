using PrinterRepair.Diagnostics.Application.Abstractions.Messaging;
using PrinterRepair.Diagnostics.Application.Repositories;

namespace PrinterRepair.Diagnostics.Application.Features.RepairTickets.Commands;

public sealed record DeleteRepairTicketCommand(Guid TicketId) : ICommand<bool>;

public sealed class DeleteRepairTicketCommandHandler : ICommandHandler<DeleteRepairTicketCommand, bool>
{
    private readonly IRepairTicketRepository _repository;

    public DeleteRepairTicketCommandHandler(IRepairTicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(
        DeleteRepairTicketCommand command,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _repository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket is null)
            return false;

        ticket.EnsureCanBeDeleted();
        await _repository.DeleteAsync(ticket, cancellationToken);

        return true;
    }
}