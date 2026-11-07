namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class UnsupportedPrinterModelException : DomainException
{
    public string ModelName { get; }

    public UnsupportedPrinterModelException(string modelName)
        : base("repair.unsupported_model", $"The printer model '{modelName}' is not supported for repairs.")
    {
        ModelName = modelName;
    }
}