using PrinterRepair.Diagnostics.Domain.Exceptions;

namespace PrinterRepair.Diagnostics.Domain.Entities;

public sealed class DiagnosticNote
{
    public Guid Id { get; }
    public string Text { get; }

    internal DiagnosticNote(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidDiagnosticNoteException();

        Id = Guid.NewGuid();
        Text = text.Trim();
    }
}