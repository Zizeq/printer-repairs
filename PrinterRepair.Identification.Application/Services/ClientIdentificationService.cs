using PrinterRepair.Identification.Domain.Aggregates;
using PrinterRepair.Identification.Domain.Exceptions;

namespace PrinterRepair.Identification.Application.Services;

public class ClientIdentificationService
{
    public ClientAccount VerifyClient(string identifier, List<ClientAccount> existingClients)
    {
        var client = existingClients.FirstOrDefault(c => c.ContactIdentifier == identifier);

        if (client == null)
            throw new UnregisteredCallerException(identifier);

        return client;
    }

    public ClientAccount RegisterClient(string contactIdentifier, bool isB2B)
    {
        return ClientAccount.RegisterNew(contactIdentifier, isB2B);
    }

    public RegisteredPrinter AssignPrinter(Guid clientAccountId, string serialNumber, string model)
    {
        return RegisteredPrinter.AssignToClient(clientAccountId, serialNumber, model);
    }
}