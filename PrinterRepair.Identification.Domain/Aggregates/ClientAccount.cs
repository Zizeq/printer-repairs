namespace PrinterRepair.Identification.Domain.Aggregates;

public class ClientAccount
{
    public Guid Id { get; private set; }
    public string ContactIdentifier { get; private set; }
    public bool IsPriorityB2B { get; private set; }

    private ClientAccount(Guid id, string contactIdentifier, bool isPriorityB2B)
    {
        Id = id;
        ContactIdentifier = contactIdentifier;
        IsPriorityB2B = isPriorityB2B;
    }

    public static ClientAccount RegisterNew(string contactIdentifier, bool isPriorityB2B = false)
    {
        if (string.IsNullOrWhiteSpace(contactIdentifier))
            throw new ArgumentException("Contact identifier cannot be empty.");

        return new ClientAccount(Guid.NewGuid(), contactIdentifier, isPriorityB2B);
    }
}