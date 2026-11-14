namespace PrinterRepair.Identification.Domain.Exceptions;

public class UnregisteredCallerException : Exception
{
    public UnregisteredCallerException(string identifier)
        : base($"No client profile found for identifier: {identifier}. Caller must be registered.")
    {
    }
}