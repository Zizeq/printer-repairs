using Microsoft.AspNetCore.Mvc;
using PrinterRepair.Diagnostics.Application.Abstractions.Messaging;
using PrinterRepair.Diagnostics.Application.Contracts;
using PrinterRepair.Diagnostics.Application.DTOs;
using PrinterRepair.Diagnostics.Application.Features.RepairTickets.Commands;
using PrinterRepair.Diagnostics.Application.Features.RepairTickets.Queries;
using PrinterRepair.Diagnostics.Domain.Exceptions;

namespace PrinterRepair.Diagnostics.Presentation.Controllers;

[ApiController]
[Route("api/tickets")]
public sealed class RepairTicketsController : ControllerBase
{
    private readonly ICommandHandler<CreateRepairTicketCommand, RepairTicketDto> _createHandler;
    private readonly ICommandHandler<IssueEstimateCommand, RepairTicketDto> _issueEstimateHandler;
    private readonly ICommandHandler<ApproveEstimateCommand, RepairTicketDto> _approveEstimateHandler;
    private readonly ICommandHandler<CompleteRepairCommand, RepairTicketDto> _completeRepairHandler;
    private readonly ICommandHandler<DeleteRepairTicketCommand, bool> _deleteHandler;
    private readonly IQueryHandler<GetRepairTicketByIdQuery, RepairTicketDto?> _getByIdHandler;
    private readonly IQueryHandler<GetAllRepairTicketsQuery, IReadOnlyList<RepairTicketDto>> _getAllHandler;

    public RepairTicketsController(
        ICommandHandler<CreateRepairTicketCommand, RepairTicketDto> createHandler,
        ICommandHandler<IssueEstimateCommand, RepairTicketDto> issueEstimateHandler,
        ICommandHandler<ApproveEstimateCommand, RepairTicketDto> approveEstimateHandler,
        ICommandHandler<CompleteRepairCommand, RepairTicketDto> completeRepairHandler,
        ICommandHandler<DeleteRepairTicketCommand, bool> deleteHandler,
        IQueryHandler<GetRepairTicketByIdQuery, RepairTicketDto?> getByIdHandler,
        IQueryHandler<GetAllRepairTicketsQuery, IReadOnlyList<RepairTicketDto>> getAllHandler)
    {
        _createHandler = createHandler;
        _issueEstimateHandler = issueEstimateHandler;
        _approveEstimateHandler = approveEstimateHandler;
        _completeRepairHandler = completeRepairHandler;
        _deleteHandler = deleteHandler;
        _getByIdHandler = getByIdHandler;
        _getAllHandler = getAllHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RepairTicketDto>>> GetAll(CancellationToken cancellationToken)
    {
        var tickets = await _getAllHandler.HandleAsync(new GetAllRepairTicketsQuery(), cancellationToken);
        return Ok(tickets);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RepairTicketDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var ticket = await _getByIdHandler.HandleAsync(new GetRepairTicketByIdQuery(id), cancellationToken);
        if (ticket is null)
            return NotFound(new { code = "diagnostics.repair_ticket_not_found", message = $"Repair ticket '{id}' was not found." });

        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<RepairTicketDto>> SubmitRequest(
        [FromBody] SubmitRepairRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateRepairTicketCommand(request.ClientId, request.PrinterModel);
            var ticket = await _createHandler.HandleAsync(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
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

    [HttpPut("{id:guid}/estimate")]
    public async Task<ActionResult<RepairTicketDto>> IssueEstimate(
        Guid id,
        [FromBody] IssueEstimateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ticket = await _issueEstimateHandler.HandleAsync(new IssueEstimateCommand(id, request.EstimatedCost), cancellationToken);
            return Ok(ticket);
        }
        catch (RepairTicketNotFoundException ex)
        {
            return NotFound(new { code = ex.Code, message = ex.Message });
        }
        catch (DomainException ex)
        {
            return UnprocessableEntity(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/approve")]
    public async Task<ActionResult<RepairTicketDto>> ApproveEstimate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var ticket = await _approveEstimateHandler.HandleAsync(new ApproveEstimateCommand(id), cancellationToken);
            return Ok(ticket);
        }
        catch (RepairTicketNotFoundException ex)
        {
            return NotFound(new { code = ex.Code, message = ex.Message });
        }
        catch (DomainException ex)
        {
            return UnprocessableEntity(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/complete")]
    public async Task<ActionResult<RepairTicketDto>> CompleteRepair(
        Guid id,
        [FromBody] CompleteRepairRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ticket = await _completeRepairHandler.HandleAsync(new CompleteRepairCommand(id, request.PassedCalibrationTest), cancellationToken);
            return Ok(ticket);
        }
        catch (RepairTicketNotFoundException ex)
        {
            return NotFound(new { code = ex.Code, message = ex.Message });
        }
        catch (DomainException ex)
        {
            return UnprocessableEntity(new { code = ex.Code, message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _deleteHandler.HandleAsync(new DeleteRepairTicketCommand(id), cancellationToken);
            if (!deleted)
                return NotFound(new { code = "diagnostics.repair_ticket_not_found", message = $"Repair ticket '{id}' was not found." });

            return NoContent();
        }
        catch (DomainException ex)
        {
            return UnprocessableEntity(new { code = ex.Code, message = ex.Message });
        }
    }
}

public sealed record IssueEstimateRequest(decimal EstimatedCost);
public sealed record CompleteRepairRequest(bool PassedCalibrationTest);