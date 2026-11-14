using Microsoft.AspNetCore.Mvc;

namespace PrinterRepair.Identification.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{

    [HttpPost]
    public IActionResult RegisterClient([FromBody] RegisterClientRequest request)
    {
        return CreatedAtAction(nameof(RegisterClient), new { id = Guid.NewGuid() }, request);
    }

    [HttpPost("{clientId}/printers")]
    public IActionResult AssignPrinter(Guid clientId, [FromBody] AssignPrinterRequest request)
    {
        return Ok();
    }
}

public record RegisterClientRequest(string ContactIdentifier, bool IsPriorityB2B);
public record AssignPrinterRequest(string SerialNumber, string PrinterModel);