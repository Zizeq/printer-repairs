using PrinterRepair.Diagnostics.Domain.Exceptions;

namespace PrinterRepair.Diagnostics.Domain.ValueObjects;

public sealed record PartQuantity
{
    public int Value { get; }

    private PartQuantity(int value) => Value = value;

    public static PartQuantity Of(int value)
    {
        if (value < 0)
            throw new InvalidQuantityException(value);

        return new PartQuantity(value);
    }

    public PartQuantity Decrease(int amount)
    {
        if (amount <= 0 || amount > Value)
            throw new InvalidQuantityException(amount);

        return new PartQuantity(Value - amount);
    }
}