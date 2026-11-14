using PrinterRepair.BAH.Domain.Aggregates;

namespace PrinterRepair.BAH.Domain.Tests;

public class ServiceInvoiceTests
{
    [Fact]
    public void Issue_WhenCostIsPositive_ShouldCreateInvoice()
    {
        var ticketId = Guid.NewGuid();
        var invoice = ServiceInvoice.Issue(ticketId, 150m);

        Assert.NotEqual(Guid.Empty, invoice.Id);
        Assert.Equal(150m, invoice.TotalAmount);
        Assert.False(invoice.IsPaid);
    }

    [Fact]
    public void Pay_WhenAmountMatchesTotal_ShouldMarkAsPaid()
    {
        var invoice = ServiceInvoice.Issue(Guid.NewGuid(), 100m);
        invoice.Pay(100m);

        Assert.True(invoice.IsPaid);
    }

    [Fact]
    public void ReleaseEquipment_WhenNotPaid_ShouldThrowException()
    {
        var invoice = ServiceInvoice.Issue(Guid.NewGuid(), 100m);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            invoice.ReleaseEquipment());

        Assert.Contains("before the invoice is paid", exception.Message);
    }

    [Fact]
    public void ApplyStorageFee_WhenOver14Days_ShouldCalculateFee()
    {
        // Arrange
        var invoice = ServiceInvoice.Issue(Guid.NewGuid(), 100m);
        decimal dailyFee = 10m;

        var futureDate = DateTime.UtcNow.AddDays(16.5);
        invoice.ApplyStorageFee(futureDate, dailyFee);

        Assert.Equal(20m, invoice.StorageFee);
        Assert.Equal(120m, invoice.TotalAmount);
    }
}