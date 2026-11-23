using System.Collections.Concurrent;
using PrinterRepair.Diagnostics.Application.Features.RepairTickets.Commands;
using PrinterRepair.Diagnostics.Application.Features.RepairTickets.Queries;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Domain.Aggregates;
using PrinterRepair.Diagnostics.Domain.Exceptions;
using Xunit;

namespace PrinterRepair.Diagnostics.Application.Tests.Features.RepairTickets;

public sealed class RepairTicketCqrsTests
{
    [Fact]
    public async Task CreateRepairTicketCommand_WhenValidInput_ShouldPersistAndReturnSubmittedDto()
    {
        // Arrange
        var repo = new FakeRepairTicketRepository();
        var handler = new CreateRepairTicketCommandHandler(repo);
        var clientId = Guid.NewGuid();

        // Act
        var result = await handler.HandleAsync(new CreateRepairTicketCommand(clientId, "HP LaserJet M404"));

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(clientId, result.ClientId);
        Assert.Equal("HP LaserJet M404", result.PrinterModel);
        Assert.Equal(TicketStatus.Submitted.ToString(), result.Status);
        Assert.Single(await repo.GetAllAsync());
    }

    [Fact]
    public async Task CreateRepairTicketCommand_WhenModelUnsupported_ShouldThrowExceptionAndNotSave()
    {
        // Arrange
        var repo = new FakeRepairTicketRepository();
        var handler = new CreateRepairTicketCommandHandler(repo);

        // Act & Assert
        await Assert.ThrowsAsync<UnsupportedPrinterModelException>(() =>
            handler.HandleAsync(new CreateRepairTicketCommand(Guid.NewGuid(), "UnknownPrinter 9000")));

        Assert.Empty(await repo.GetAllAsync());
    }

    [Fact]
    public async Task IssueEstimateCommand_WhenValidCost_ShouldUpdateAndPersistTicket()
    {
        // Arrange
        var repo = new FakeRepairTicketRepository();
        var created = await new CreateRepairTicketCommandHandler(repo)
            .HandleAsync(new CreateRepairTicketCommand(Guid.NewGuid(), "HP LaserJet Pro"));

        var handler = new IssueEstimateCommandHandler(repo);

        // Act
        var result = await handler.HandleAsync(new IssueEstimateCommand(created.Id, 500m));

        // Assert
        Assert.Equal(500m, result.EstimatedCost);
        Assert.Equal(TicketStatus.EstimateIssued.ToString(), result.Status);
    }

    [Fact]
    public async Task GetRepairTicketByIdQuery_WhenTicketExists_ShouldReturnDto()
    {
        // Arrange
        var repo = new FakeRepairTicketRepository();
        var created = await new CreateRepairTicketCommandHandler(repo)
            .HandleAsync(new CreateRepairTicketCommand(Guid.NewGuid(), "HP LaserJet Pro"));

        var queryHandler = new GetRepairTicketByIdQueryHandler(repo);

        // Act
        var result = await queryHandler.HandleAsync(new GetRepairTicketByIdQuery(created.Id));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("HP LaserJet Pro", result.PrinterModel);
    }

    [Fact]
    public async Task GetRepairTicketByIdQuery_WhenTicketDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var repo = new FakeRepairTicketRepository();
        var queryHandler = new GetRepairTicketByIdQueryHandler(repo);

        // Act
        var result = await queryHandler.HandleAsync(new GetRepairTicketByIdQuery(Guid.NewGuid()));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllRepairTicketsQuery_WhenTicketsExist_ShouldReturnAllDtos()
    {
        // Arrange
        var repo = new FakeRepairTicketRepository();
        var createHandler = new CreateRepairTicketCommandHandler(repo);
        await createHandler.HandleAsync(new CreateRepairTicketCommand(Guid.NewGuid(), "Brother HL-L2350DW"));
        await createHandler.HandleAsync(new CreateRepairTicketCommand(Guid.NewGuid(), "Canon i-SENSYS LBP6030"));

        var queryHandler = new GetAllRepairTicketsQueryHandler(repo);

        // Act
        var results = await queryHandler.HandleAsync(new GetAllRepairTicketsQuery());

        // Assert
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task DeleteRepairTicketCommand_WhenTicketIsSubmitted_ShouldRemoveTicketAndReturnTrue()
    {
        // Arrange
        var repo = new FakeRepairTicketRepository();
        var created = await new CreateRepairTicketCommandHandler(repo)
            .HandleAsync(new CreateRepairTicketCommand(Guid.NewGuid(), "Epson EcoTank L3250"));

        var deleteHandler = new DeleteRepairTicketCommandHandler(repo);

        // Act
        var deleted = await deleteHandler.HandleAsync(new DeleteRepairTicketCommand(created.Id));
        var afterDelete = await repo.GetByIdReadOnlyAsync(created.Id);

        // Assert
        Assert.True(deleted);
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task DeleteRepairTicketCommand_WhenEstimateIsApproved_ShouldThrowDeletionForbiddenException()
    {
        // Arrange
        var repo = new FakeRepairTicketRepository();
        var created = await new CreateRepairTicketCommandHandler(repo)
            .HandleAsync(new CreateRepairTicketCommand(Guid.NewGuid(), "HP LaserJet Pro M428"));

        await new ApproveEstimateCommandHandler(repo)
            .HandleAsync(new ApproveEstimateCommand(created.Id));

        var deleteHandler = new DeleteRepairTicketCommandHandler(repo);

        // Act & Assert
        await Assert.ThrowsAsync<RepairTicketDeletionForbiddenException>(() =>
            deleteHandler.HandleAsync(new DeleteRepairTicketCommand(created.Id)));
    }

    private sealed class FakeRepairTicketRepository : IRepairTicketRepository
    {
        private readonly ConcurrentDictionary<Guid, RepairTicket> _store = new();

        public Task<RepairTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_store.GetValueOrDefault(id));

        public Task<RepairTicket?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_store.GetValueOrDefault(id));

        public Task<IReadOnlyList<RepairTicket>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<RepairTicket>>(_store.Values.ToList());

        public Task SaveAsync(RepairTicket ticket, CancellationToken cancellationToken = default)
        {
            _store[ticket.Id] = ticket;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(RepairTicket ticket, CancellationToken cancellationToken = default)
        {
            _store.TryRemove(ticket.Id, out _);
            return Task.CompletedTask;
        }
    }
}