using PrinterRepair.BAH.Domain.Aggregates;

namespace PrinterRepair.BAH.Application.Services;

public class ServiceInvoiceService
{
    public ServiceInvoice IssueInvoice(Guid ticketId, decimal totalCost)
    {
        return ServiceInvoice.Issue(ticketId, totalCost);
    }

    public void ProcessPayment(ServiceInvoice invoice, decimal paymentAmount)
    {
        invoice.Pay(paymentAmount);
    }

    public void ReleasePrinter(ServiceInvoice invoice)
    {
        invoice.ReleaseEquipment();
    }

    public void CalculateDailyStorageFees(ServiceInvoice invoice, DateTime currentDate)
    {
        invoice.ApplyStorageFee(currentDate, 10m);
    }
}