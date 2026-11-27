using PrinterRepair.Diagnostics.Application.Abstractions.Messaging;
using PrinterRepair.Diagnostics.Application.DTOs;
using PrinterRepair.Diagnostics.Application.Repositories;

namespace PrinterRepair.Diagnostics.Application.Features.RepairTickets.Queries;

public sealed record GetRepairTicketByIdQuery(Guid TicketId) : IQuery<RepairTicketDto?>;

internal sealed class GetRepairTicketByIdQueryHandler : IQueryHandler<GetRepairTicketByIdQuery, RepairTicketDto?>
{
    private readonly IRepairTicketRepository _repository;

    public GetRepairTicketByIdQueryHandler(IRepairTicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<RepairTicketDto?> HandleAsync(
        GetRepairTicketByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _repository.GetByIdReadOnlyAsync(query.TicketId, cancellationToken);
        return ticket is null ? null : RepairTicketDto.FromDomain(ticket);
    }
}