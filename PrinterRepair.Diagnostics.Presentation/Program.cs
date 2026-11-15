using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Application.Services;
using PrinterRepair.Diagnostics.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IRepairTicketService, RepairTicketService>();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

await app.Services.InitializeDatabaseAsync();

app.Run();