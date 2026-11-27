namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class CalibrationFailedException : DomainException
{
    public Guid TicketId { get; }

    public CalibrationFailedException(Guid ticketId)
        : base(
            code: "diagnostics.calibration_failed",
            message: $"Repair ticket '{ticketId}' cannot be completed because the final calibration test failed.")
    {
        TicketId = ticketId;
    }
}
