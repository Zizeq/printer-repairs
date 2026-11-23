using PrinterRepair.Diagnostics.Domain.ValueObjects;

namespace PrinterRepair.Diagnostics.Application.Ports;

public sealed record SupplierOrderRef(string ExternalOrderId);

public sealed class PartsSupplierIntegrationException(string message, Exception? innerException = null)
    : Exception(message, innerException);

public interface IPartsSupplierPort
{
    Task<SupplierOrderRef> OrderPartAsync(
        string partCode,
        PartQuantity quantity,
        Money maxUnitPrice,
        CancellationToken cancellationToken = default);
}