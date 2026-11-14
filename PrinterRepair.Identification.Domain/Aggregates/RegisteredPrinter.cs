namespace PrinterRepair.Identification.Domain.Aggregates;

public class RegisteredPrinter
{
    public Guid Id { get; private set; }
    public Guid ClientAccountId { get; private set; }
    public string SerialNumber { get; private set; }
    public string PrinterModel { get; private set; }

    private RegisteredPrinter(Guid id, Guid clientAccountId, string serialNumber, string printerModel)
    {
        Id = id;
        ClientAccountId = clientAccountId;
        SerialNumber = serialNumber;
        PrinterModel = printerModel;
    }

    public static RegisteredPrinter AssignToClient(Guid clientAccountId, string serialNumber, string printerModel)
    {
        if (string.IsNullOrWhiteSpace(serialNumber) || string.IsNullOrWhiteSpace(printerModel))
            throw new ArgumentException("Printer details cannot be empty.");

        return new RegisteredPrinter(Guid.NewGuid(), clientAccountId, serialNumber, printerModel);
    }
}