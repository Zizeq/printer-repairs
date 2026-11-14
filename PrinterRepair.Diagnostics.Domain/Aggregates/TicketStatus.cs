namespace PrinterRepair.Diagnostics.Domain.Aggregates;

public enum TicketStatus
{
    Submitted,
    EstimateIssued,
    EstimateApproved,
    Rejected,
    Completed
}