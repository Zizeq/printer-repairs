using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PrinterRepair.Diagnostics.Application.Abstractions.Messaging;
using PrinterRepair.Diagnostics.Application.DTOs;
using PrinterRepair.Diagnostics.Application.Features.RepairTickets.Commands;
using PrinterRepair.Diagnostics.Application.Features.RepairTickets.Queries;
using PrinterRepair.Diagnostics.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// CQRS Command Handlers (Write Path)
builder.Services.AddScoped<ICommandHandler<CreateRepairTicketCommand, RepairTicketDto>, CreateRepairTicketCommandHandler>();
builder.Services.AddScoped<ICommandHandler<IssueEstimateCommand, RepairTicketDto>, IssueEstimateCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ApproveEstimateCommand, RepairTicketDto>, ApproveEstimateCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CompleteRepairCommand, RepairTicketDto>, CompleteRepairCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteRepairTicketCommand, bool>, DeleteRepairTicketCommandHandler>();

// CQRS Query Handlers (Read Path)
builder.Services.AddScoped<IQueryHandler<GetRepairTicketByIdQuery, RepairTicketDto?>, GetRepairTicketByIdQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetAllRepairTicketsQuery, IReadOnlyList<RepairTicketDto>>, GetAllRepairTicketsQueryHandler>();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

await app.Services.InitializeDatabaseAsync();

app.Run();