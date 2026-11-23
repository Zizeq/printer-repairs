namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class RepairTicketDeletionForbiddenException : DomainException
{
    public RepairTicketDeletionForbiddenException(Guid ticketId, string currentStatus)
        : base(
            code: "diagnostics.repair_ticket_deletion_forbidden",
            message: $"Repair ticket '{ticketId}' cannot be deleted while in status '{currentStatus}'.")
    {
    }
}