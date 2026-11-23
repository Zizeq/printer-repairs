namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class ObsoletePartException : DomainException
{
    public ObsoletePartException(string partCode)
        : base("diagnostics.obsolete_part", $"Part '{partCode}' is permanently discontinued.") { }
}