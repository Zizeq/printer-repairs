using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Infrastructure.Persistence;
using PrinterRepair.Diagnostics.Application.Ports;
using PrinterRepair.Diagnostics.Infrastructure.Acl;
using System;

namespace PrinterRepair.Diagnostics.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var useInMemory = configuration.GetValue<bool>("Persistence:UseInMemoryDatabase");

        services.AddDbContext<DiagnosticsDbContext>(options =>
        {
            if (useInMemory)
            {
                var dbName = configuration["Persistence:InMemoryDatabaseName"] ?? "DiagnosticsLabDb";
                options.UseInMemoryDatabase(dbName);
            }
            else
            {
                var connectionString = configuration.GetConnectionString("DiagnosticsDb")
                    ?? throw new InvalidOperationException("Missing connection string 'DiagnosticsDb'.");

                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "diagnostics");
                });
            }
        });

        services.AddScoped<IRepairTicketRepository, Repositories.RepairTicketRepository>();

        services.AddScoped<PartsSupplierTranslator>();
        services.AddScoped(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var baseUrl = config["PartsSupplier:BaseUrl"] ?? "https://legacy-supplier.example.local/";
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            return new HttpPartsSupplierAdapter(httpClient);
        });
        services.AddScoped<IPartsSupplierPort, LegacyPartsSupplierAcl>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DiagnosticsDbContext>();

        await DiagnosticsDataSeeder.SeedAsync(context);
    }
}