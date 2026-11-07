using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Application.Services;
using PrinterRepair.Diagnostics.Presentation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IRepairTicketService, RepairTicketService>();

builder.Services.AddSingleton<IRepairTicketRepository, InMemoryRepairTicketRepository>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();