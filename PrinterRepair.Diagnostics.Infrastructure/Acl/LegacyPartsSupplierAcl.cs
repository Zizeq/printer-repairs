using PrinterRepair.Diagnostics.Application.Ports;
using PrinterRepair.Diagnostics.Domain.ValueObjects;

namespace PrinterRepair.Diagnostics.Infrastructure.Acl;

internal sealed class LegacyPartsSupplierAcl : IPartsSupplierPort
{
    private readonly HttpPartsSupplierAdapter _adapter;
    private readonly PartsSupplierTranslator _translator;

    public LegacyPartsSupplierAcl(
        HttpPartsSupplierAdapter adapter,
        PartsSupplierTranslator translator)
    {
        _adapter = adapter;
        _translator = translator;
    }

    public async Task<SupplierOrderRef> OrderPartAsync(
        string partCode,
        PartQuantity quantity,
        Money maxUnitPrice,
        CancellationToken cancellationToken = default)
    {
        var requestDto = _translator.ToSupplierDto(partCode, quantity, maxUnitPrice);
        var responseDto = await _adapter.SubmitOrderAsync(requestDto, cancellationToken);

        if (responseDto.ERR_CD != 0)
            throw _translator.ToException(responseDto.ERR_CD, partCode);

        return _translator.ToSupplierOrderRef(responseDto);
    }
}