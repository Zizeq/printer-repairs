using Microsoft.AspNetCore.Mvc;
using PrinterRepair.Diagnostics.Application.Contracts;
using PrinterRepair.Diagnostics.Application.DTOs;
using PrinterRepair.Diagnostics.Application.Services;
using PrinterRepair.Diagnostics.Domain.Exceptions;

namespace PrinterRepair.Diagnostics.Presentation.Controllers;

[ApiController]
[Route("api/tickets")]
public sealed class RepairTicketsController : ControllerBase
{
    private readonly IRepairTicketService _service;

    public RepairTicketsController(IRepairTicketService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<RepairTicketDto>> SubmitRequest(
        [FromBody] SubmitRepairRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ticket = await _service.SubmitRequestAsync(request.ClientId, request.PrinterModel, cancellationToken);
            return CreatedAtAction(nameof(SubmitRequest), new { id = ticket.Id }, ticket);
        }
        catch (DomainException ex)
        {
            return UnprocessableEntity(new { code = ex.Code, message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { code = "validation.invalid_argument", message = ex.Message });
        }
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveEstimate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _service.ApproveEstimateAsync(id, cancellationToken);
            return Ok();
        }
        catch (DomainException ex)
        {
            return UnprocessableEntity(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteRepair(Guid id, [FromBody] CompleteRepairRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _service.CompleteRepairAsync(id, request.PassedCalibrationTest, cancellationToken);
            return Ok();
        }
        catch (DomainException ex)
        {
            return UnprocessableEntity(new { code = ex.Code, message = ex.Message });
        }
    }
}

public record CompleteRepairRequest(bool PassedCalibrationTest);