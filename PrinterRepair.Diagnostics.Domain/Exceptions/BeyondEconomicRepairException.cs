namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class BeyondEconomicRepairException : DomainException
{
    public decimal EstimatedCost { get; }

    public BeyondEconomicRepairException(decimal estimatedCost)
        : base("repair.beyond_economic_repair", $"Repair cost of {estimatedCost:C} exceeds the printer's economic value.")
    {
        EstimatedCost = estimatedCost;
    }
}