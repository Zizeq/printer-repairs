using System;
using System.Collections.Generic;
using System.Linq;
using PrinterRepair.Diagnostics.Domain.Entities;
using PrinterRepair.Diagnostics.Domain.Exceptions;

namespace PrinterRepair.Diagnostics.Domain.Aggregates;

public sealed class RepairTicket
{
    private readonly string[] _unsupportedModels = { "ObsoleteBrand X1", "UnknownPrinter 9000" };
    private const decimal MaxEconomicRepairCost = 1000m;

    private readonly List<DiagnosticNote> _notes = new();

    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public string PrinterModel { get; private set; }
    public decimal? RepairCostEstimate { get; private set; }
    public TicketStatus Status { get; private set; }

    public IReadOnlyCollection<DiagnosticNote> Notes => _notes.AsReadOnly();

    private RepairTicket()
    {
        PrinterModel = null!;
    }

    public RepairTicket(Guid clientId, string printerModel)
    {
        if (string.IsNullOrWhiteSpace(printerModel))
            throw new ArgumentException("Printer model cannot be empty.", nameof(printerModel));

        if (_unsupportedModels.Contains(printerModel))
            throw new UnsupportedPrinterModelException(printerModel);

        Id = Guid.NewGuid();
        ClientId = clientId;
        PrinterModel = printerModel.Trim();
        Status = TicketStatus.Submitted;
    }

    public void IssueEstimate(decimal estimatedCost)
    {
        if (estimatedCost > MaxEconomicRepairCost)
        {
            Status = TicketStatus.Rejected;
            throw new BeyondEconomicRepairException(estimatedCost);
        }

        RepairCostEstimate = estimatedCost;
        Status = TicketStatus.EstimateIssued;
    }

    public void AddDiagnosticNote(string text)
    {
        _notes.Add(new DiagnosticNote(text));
    }

    public void ApproveEstimate()
    {
        if (Status == TicketStatus.Completed)
            throw new InvalidOperationException("Cannot approve an estimate for an already completed repair.");

        Status = TicketStatus.EstimateApproved;
    }

    public void CompleteRepair(bool passedCalibrationTest)
    {
        if (!passedCalibrationTest)
            throw new InvalidOperationException("Repair failed: Calibration test did not pass.");

        Status = TicketStatus.Completed;
    }
    public void EnsureCanBeDeleted()
    {
        if (Status is TicketStatus.EstimateApproved or TicketStatus.Completed)
            throw new RepairTicketDeletionForbiddenException(Id, Status.ToString());
    }
}