using System.Globalization;
using PrinterRepair.Diagnostics.Application.Ports;
using PrinterRepair.Diagnostics.Domain.Exceptions;
using PrinterRepair.Diagnostics.Domain.ValueObjects;
using PrinterRepair.Diagnostics.Infrastructure.Acl.ExternalContracts;

namespace PrinterRepair.Diagnostics.Infrastructure.Acl;

internal sealed class PartsSupplierTranslator
{
    public PART_REQ_DTO ToSupplierDto(string partCode, PartQuantity quantity, Money maxUnitPrice)
    {
        if (string.IsNullOrWhiteSpace(partCode))
            throw new PartsSupplierIntegrationException("Part code cannot be empty.");

        if (maxUnitPrice.Currency != "PLN")
            throw new PartsSupplierIntegrationException(
                $"Legacy parts supplier accepts only PLN, got '{maxUnitPrice.Currency}'.");

        return new PART_REQ_DTO
        {
            PART_CD = partCode.Trim().ToUpperInvariant(),
            QTY_REQ = quantity.Value,
            MAX_PRC_S = maxUnitPrice.Amount.ToString("F2", CultureInfo.InvariantCulture)
        };
    }

    public SupplierOrderRef ToSupplierOrderRef(PART_RES_DTO response)
    {
        if (string.IsNullOrWhiteSpace(response.EXT_ORD_NO))
            throw new PartsSupplierIntegrationException(
                "Legacy parts supplier response did not contain an external order number.");

        return new SupplierOrderRef(response.EXT_ORD_NO);
    }

    public Exception ToException(int errorCode, string partCode) => errorCode switch
    {
        -10 => new InsufficientStockException(partCode),
        -20 => new ObsoletePartException(partCode),
        _ => new PartsSupplierIntegrationException(
                   $"Legacy parts supplier rejected order for '{partCode}' with ERR_CD={errorCode}.")
    };
}