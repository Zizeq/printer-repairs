namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class InvalidPriceException : DomainException
{
    public InvalidPriceException(decimal amount)
        : base("diagnostics.invalid_price", $"Monetary amount '{amount}' cannot be negative.") { }
}