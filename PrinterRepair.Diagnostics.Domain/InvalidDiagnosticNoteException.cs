namespace PrinterRepair.Diagnostics.Domain.Exceptions;

public sealed class InvalidDiagnosticNoteException : DomainException
{
    public InvalidDiagnosticNoteException()
        : base("repair.invalid_diagnostic_note", "Diagnostic note cannot be empty.")
    {
    }
}