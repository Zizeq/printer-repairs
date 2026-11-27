namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class InvalidRepairTicketStateException : DomainException
{
    public Guid TicketId { get; }
    public string CurrentStatus { get; }
    public string AttemptedOperation { get; }

    public InvalidRepairTicketStateException(Guid ticketId, string currentStatus, string attemptedOperation)
        : base(
            code: "diagnostics.invalid_ticket_state",
            message: $"Operation '{attemptedOperation}' is forbidden for repair ticket '{ticketId}' while in status '{currentStatus}'.")
    {
        TicketId = ticketId;
        CurrentStatus = currentStatus;
        AttemptedOperation = attemptedOperation;
    }
}
