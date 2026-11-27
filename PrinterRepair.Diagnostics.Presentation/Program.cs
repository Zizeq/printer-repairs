using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PrinterRepair.Diagnostics.Application.Abstractions.Messaging;
using PrinterRepair.Diagnostics.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var applicationAssembly = typeof(ICommand<>).Assembly;
foreach (var implementationType in applicationAssembly.GetTypes().Where(t => t is { IsClass: true, IsAbstract: false }))
{
    var handlerInterfaces = implementationType.GetInterfaces()
        .Where(i => i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                     i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

    foreach (var handlerInterface in handlerInterfaces)
    {
        builder.Services.AddScoped(handlerInterface, implementationType);
    }
}

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    await app.Services.InitializeDatabaseAsync();
}

app.Run();