namespace PrinterRepair.Diagnostics.Infrastructure.Acl.ExternalContracts;

internal sealed class PART_REQ_DTO
{
    public string PART_CD { get; init; } = string.Empty;
    public int QTY_REQ { get; init; }
    public string MAX_PRC_S { get; init; } = string.Empty;
}