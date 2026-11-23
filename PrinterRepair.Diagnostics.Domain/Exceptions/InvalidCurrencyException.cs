namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class InvalidCurrencyException : DomainException
{
    public InvalidCurrencyException(string currency)
        : base("diagnostics.invalid_currency", $"Currency code '{currency}' must be a 3-letter ISO code.") { }
}