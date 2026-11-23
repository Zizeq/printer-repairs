namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class CurrencyMismatchException : DomainException
{
    public CurrencyMismatchException(string expected, string actual)
        : base("diagnostics.currency_mismatch", $"Cannot combine currencies '{expected}' and '{actual}'.") { }
}