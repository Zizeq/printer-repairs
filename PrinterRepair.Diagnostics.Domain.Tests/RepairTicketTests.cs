using PrinterRepair.Diagnostics.Domain.Aggregates;
using PrinterRepair.Diagnostics.Domain.Exceptions;
using Xunit;

namespace PrinterRepair.Diagnostics.Domain.Tests;

public class RepairTicketTests
{
    [Fact]
    public void SubmitRequest_WhenValidModel_CreatesTicketSuccessfully()
    {
        var ticket = new RepairTicket(Guid.NewGuid(), "HP LaserJet Pro");

        Assert.Equal("HP LaserJet Pro", ticket.PrinterModel);
    }

    [Theory]
    [InlineData("ObsoleteBrand X1")]
    [InlineData("UnknownPrinter 9000")]
    public void SubmitRequest_WhenModelUnsupported_ThrowsUnsupportedPrinterModelException(string invalidModel)
    {
        var ex = Assert.Throws<UnsupportedPrinterModelException>(() =>
            new RepairTicket(Guid.NewGuid(), invalidModel));

        Assert.Equal("repair.unsupported_model", ex.Code);
        Assert.Equal(invalidModel, ex.ModelName);
    }
    [Fact]
    public void IssueEstimate_WhenCostTooHigh_ThrowsBeyondEconomicRepairException()
    {
        var ticket = new RepairTicket(Guid.NewGuid(), "Epson EcoTank");

        var ex = Assert.Throws<BeyondEconomicRepairException>(() =>
            ticket.IssueEstimate(1500m));

        Assert.Equal("repair.beyond_economic_repair", ex.Code);
        Assert.Equal(1500m, ex.EstimatedCost);
        Assert.Equal(TicketStatus.Rejected, ticket.Status);
    }
    [Fact]
    public void AddDiagnosticNote_WhenValidText_AddsToCollection()
    {
        var ticket = new RepairTicket(Guid.NewGuid(), "HP LaserJet Pro");

        ticket.AddDiagnosticNote("Printer head is jammed.");

        Assert.Single(ticket.Notes);
        Assert.Equal("Printer head is jammed.", ticket.Notes.First().Text);
    }

    [Fact]
    public void AddDiagnosticNote_WhenTextEmpty_ThrowsInvalidDiagnosticNoteException()
    {
        var ticket = new RepairTicket(Guid.NewGuid(), "HP LaserJet Pro");

        var ex = Assert.Throws<InvalidDiagnosticNoteException>(() => ticket.AddDiagnosticNote(""));
        Assert.Equal("repair.invalid_diagnostic_note", ex.Code);
    }
    [Fact]
    public void ApproveEstimate_WhenValid_ShouldChangeStatusToEstimateApproved()
    {
        var ticket = new RepairTicket(Guid.NewGuid(), "ValidModel");
        ticket.ApproveEstimate();

        Assert.Equal(TicketStatus.EstimateApproved, ticket.Status);
    }

    [Fact]
    public void CompleteRepair_WhenCalibrationPasses_ShouldChangeStatusToCompleted()
    {
        var ticket = new RepairTicket(Guid.NewGuid(), "ValidModel");
        ticket.CompleteRepair(true);

        Assert.Equal(TicketStatus.Completed, ticket.Status);
    }

    [Fact]
    public void CompleteRepair_WhenCalibrationFails_ShouldThrowException()
    {
        var ticket = new RepairTicket(Guid.NewGuid(), "ValidModel");

        var exception = Assert.Throws<InvalidOperationException>(() =>
            ticket.CompleteRepair(false));

        Assert.Contains("Calibration test did not pass", exception.Message);
    }
}