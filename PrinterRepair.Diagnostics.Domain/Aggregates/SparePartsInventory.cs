namespace PrinterRepair.DiagnosticsAndRepair.Domain.Aggregates;

public class SparePartsInventory
{
    public Guid PartId { get; private set; }
    public string PartName { get; private set; }
    public int StockLevel { get; private set; }

    public SparePartsInventory(Guid partId, string partName, int initialStock)
    {
        PartId = partId;
        PartName = partName;
        StockLevel = initialStock;
    }

    public void ReservePart(int quantity)
    {
        if (StockLevel < quantity)
            throw new InvalidOperationException($"Part {PartName} is out of stock. Please order from supplier.");

        StockLevel -= quantity;
    }
}