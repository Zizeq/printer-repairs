namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class InsufficientStockException : DomainException
{
    public InsufficientStockException(string partCode)
        : base("diagnostics.insufficient_stock", $"Insufficient stock available for part '{partCode}'.") { }
}