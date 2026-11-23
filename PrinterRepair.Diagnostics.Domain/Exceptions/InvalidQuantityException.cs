namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class InvalidQuantityException : DomainException
{
    public InvalidQuantityException(int quantity)
        : base("diagnostics.invalid_quantity", $"Part quantity '{quantity}' is invalid.") { }
}