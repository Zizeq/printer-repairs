using Microsoft.AspNetCore.Mvc;

namespace PrinterRepair.BAH.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    [HttpPost]
    public IActionResult IssueInvoice([FromBody] IssueInvoiceRequest request)
    {
        return CreatedAtAction(nameof(IssueInvoice), new { id = Guid.NewGuid() }, request);
    }

    [HttpPut("{invoiceId}/pay")]
    public IActionResult PayInvoice(Guid invoiceId, [FromBody] PayInvoiceRequest request)
    {
        return Ok();
    }
}

public record IssueInvoiceRequest(Guid RepairTicketId, decimal RepairCost);
public record PayInvoiceRequest(decimal PaymentAmount);