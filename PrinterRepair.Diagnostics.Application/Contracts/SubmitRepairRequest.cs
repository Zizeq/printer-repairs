namespace PrinterRepair.Diagnostics.Application.Contracts;

public sealed record SubmitRepairRequest(Guid ClientId, string PrinterModel);