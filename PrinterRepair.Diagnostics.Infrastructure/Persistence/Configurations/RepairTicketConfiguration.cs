using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrinterRepair.Diagnostics.Domain.Aggregates;

namespace PrinterRepair.Diagnostics.Infrastructure.Persistence.Configurations;

internal sealed class RepairTicketConfiguration : IEntityTypeConfiguration<RepairTicket>
{
    public void Configure(EntityTypeBuilder<RepairTicket> builder)
    {
        builder.ToTable("RepairTickets", schema: "diagnostics");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.ClientId)
            .IsRequired();

        builder.Property(t => t.PrinterModel)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.RepairCostEstimate)
            .HasPrecision(18, 2);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.OwnsMany(t => t.Notes, noteBuilder =>
        {
            noteBuilder.ToTable("DiagnosticNotes", schema: "diagnostics");
            noteBuilder.WithOwner().HasForeignKey("RepairTicketId");
        });

        builder.Navigation(t => t.Notes)
            .HasField("_notes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}