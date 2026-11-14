namespace PrinterRepair.BAH.Domain.Aggregates;

public class ServiceInvoice
{
    public Guid Id { get; private set; }
    public Guid RepairTicketId { get; private set; }
    public decimal RepairCost { get; private set; }
    public decimal StorageFee { get; private set; }
    public decimal TotalAmount => RepairCost + StorageFee;

    public bool IsPaid { get; private set; }
    public bool IsReleased { get; private set; }
    public DateTime IssuedAt { get; private set; }

    private ServiceInvoice(Guid id, Guid repairTicketId, decimal repairCost, DateTime issuedAt)
    {
        Id = id;
        RepairTicketId = repairTicketId;
        RepairCost = repairCost;
        IssuedAt = issuedAt;
        StorageFee = 0;
        IsPaid = false;
        IsReleased = false;
    }

    public static ServiceInvoice Issue(Guid repairTicketId, decimal repairCost)
    {
        if (repairCost <= 0)
            throw new ArgumentException("Repair cost must be greater than zero.");

        return new ServiceInvoice(Guid.NewGuid(), repairTicketId, repairCost, DateTime.UtcNow);
    }

    public void Pay(decimal paymentAmount)
    {
        if (paymentAmount != TotalAmount)
            throw new InvalidOperationException("Payment rejected: Amount must exactly match the total invoice amount.");

        IsPaid = true;
    }

    public void ReleaseEquipment()
    {
        if (!IsPaid)
            throw new InvalidOperationException("Cannot release equipment before the invoice is paid.");

        IsReleased = true;
    }

    public void ApplyStorageFee(DateTime currentDate, decimal dailyFeeRate)
    {
        var daysUnclaimed = (currentDate - IssuedAt).TotalDays;

        if (daysUnclaimed > 14 && !IsReleased)
        {
            var chargeableDays = (int)Math.Floor(daysUnclaimed - 14);
            StorageFee = chargeableDays * dailyFeeRate;
        }
    }
}