namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class RepairTicketNotFoundException : DomainException
{
    public RepairTicketNotFoundException(Guid ticketId)
        : base(
            code: "diagnostics.repair_ticket_not_found",
            message: $"Repair ticket '{ticketId}' was not found.")
    {
    }
}