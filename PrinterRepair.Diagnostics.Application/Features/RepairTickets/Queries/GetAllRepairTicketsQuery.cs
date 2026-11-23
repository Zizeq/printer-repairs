using PrinterRepair.Diagnostics.Application.Abstractions.Messaging;
using PrinterRepair.Diagnostics.Application.DTOs;
using PrinterRepair.Diagnostics.Application.Repositories;

namespace PrinterRepair.Diagnostics.Application.Features.RepairTickets.Queries;

public sealed record GetAllRepairTicketsQuery : IQuery<IReadOnlyList<RepairTicketDto>>;

public sealed class GetAllRepairTicketsQueryHandler : IQueryHandler<GetAllRepairTicketsQuery, IReadOnlyList<RepairTicketDto>>
{
    private readonly IRepairTicketRepository _repository;

    public GetAllRepairTicketsQueryHandler(IRepairTicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<RepairTicketDto>> HandleAsync(
        GetAllRepairTicketsQuery query,
        CancellationToken cancellationToken = default)
    {
        var tickets = await _repository.GetAllAsync(cancellationToken);
        return tickets.Select(RepairTicketDto.FromDomain).ToList();
    }
}