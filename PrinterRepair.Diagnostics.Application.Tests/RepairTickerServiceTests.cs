using Moq;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Application.Services;
using PrinterRepair.Diagnostics.Domain.Aggregates;
using PrinterRepair.Diagnostics.Domain.Exceptions;
using Xunit;

namespace PrinterRepair.Diagnostics.Application.Tests;

public class RepairTicketServiceTests
{
    private readonly Mock<IRepairTicketRepository> _repositoryMock = new();
    private readonly RepairTicketService _sut;

    public RepairTicketServiceTests()
    {
        _sut = new RepairTicketService(_repositoryMock.Object);
    }

    [Fact]
    public async Task SubmitRequestAsync_WhenValidModel_SavesTicketAndReturnsDto()
    {
        var clientId = Guid.NewGuid();

        var result = await _sut.SubmitRequestAsync(clientId, "Epson WorkForce", CancellationToken.None);

        Assert.Equal(clientId, result.ClientId);
        Assert.Equal("Epson WorkForce", result.PrinterModel);
        Assert.Equal(TicketStatus.Submitted.ToString(), result.Status);

        _repositoryMock.Verify(r => r.SaveAsync(It.Is<RepairTicket>(t => t.Id == result.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SubmitRequestAsync_WhenModelUnsupported_ThrowsExceptionAndNeverSaves()
    {
        var clientId = Guid.NewGuid();

        await Assert.ThrowsAsync<UnsupportedPrinterModelException>(() =>
            _sut.SubmitRequestAsync(clientId, "UnknownPrinter 9000", CancellationToken.None));

        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<RepairTicket>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    [Fact]
    public async Task GetTicketAsync_WhenTicketExists_ReturnsDto()
    {
        var ticket = new RepairTicket(Guid.NewGuid(), "HP LaserJet Pro");
        _repositoryMock.Setup(r => r.GetByIdAsync(ticket.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ticket);

        var result = await _sut.GetTicketAsync(ticket.Id);

        Assert.NotNull(result);
        Assert.Equal(ticket.Id, result.Id);
    }

    [Fact]
    public async Task IssueEstimateAsync_WhenValidCost_UpdatesAndSavesTicket()
    {
        var ticket = new RepairTicket(Guid.NewGuid(), "HP LaserJet Pro");
        _repositoryMock.Setup(r => r.GetByIdAsync(ticket.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ticket);

        var result = await _sut.IssueEstimateAsync(ticket.Id, 500m);

        Assert.Equal(500m, result.EstimatedCost);
        Assert.Equal(TicketStatus.EstimateIssued.ToString(), result.Status);
        _repositoryMock.Verify(r => r.SaveAsync(ticket, It.IsAny<CancellationToken>()), Times.Once);
    }
}