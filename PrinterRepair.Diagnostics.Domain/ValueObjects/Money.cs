using PrinterRepair.Diagnostics.Domain.Exceptions;

namespace PrinterRepair.Diagnostics.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.ToEven);
        Currency = currency;
    }

    public static Money Of(decimal amount, string currency)
    {
        if (amount < 0m)
            throw new InvalidPriceException(amount);

        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
            throw new InvalidCurrencyException(currency ?? string.Empty);

        return new Money(amount, currency.Trim().ToUpperInvariant());
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new CurrencyMismatchException(Currency, other.Currency);

        return new Money(Amount + other.Amount, Currency);
    }
}