using System.Net.Http.Json;
using PrinterRepair.Diagnostics.Application.Ports;
using PrinterRepair.Diagnostics.Infrastructure.Acl.ExternalContracts;

namespace PrinterRepair.Diagnostics.Infrastructure.Acl;

internal sealed class HttpPartsSupplierAdapter
{
    private readonly HttpClient _httpClient;

    public HttpPartsSupplierAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PART_RES_DTO> SubmitOrderAsync(
        PART_REQ_DTO request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(
                "legacy/parts/order",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<PART_RES_DTO>(cancellationToken);
            return payload ?? throw new PartsSupplierIntegrationException(
                "Legacy parts supplier returned an empty response payload.");
        }
        catch (PartsSupplierIntegrationException)
        {
            throw;
        }
        catch (Exception ex) when (
            ex is HttpRequestException or TimeoutException ||
            (ex is TaskCanceledException && !cancellationToken.IsCancellationRequested))
        {
            throw new PartsSupplierIntegrationException(
                "Legacy parts supplier API is unavailable or timed out.",
                ex);
        }
    }
}