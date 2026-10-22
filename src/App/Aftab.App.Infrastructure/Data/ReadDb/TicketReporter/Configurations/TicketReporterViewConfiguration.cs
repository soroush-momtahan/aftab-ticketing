using Aftab.App.Application.Ticketing.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aftab.App.Infrastructure.Data.ReadDb.TicketReporter.Configurations;

internal sealed class TicketReporterViewConfiguration : IEntityTypeConfiguration<TicketReporterView>
{
    public void Configure(EntityTypeBuilder<TicketReporterView> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(50);

        builder.Property(x => x.OrganId)
            .HasMaxLength(50);
        
        builder.Property(x => x.OrganName)
            .HasDefaultValue("آفتاب آسا بخش تستی");

        builder.Ignore(x => x.TotalStatusCount);
        builder.Ignore(x => x.OpenTickets);
        builder.Ignore(x => x.InprogressTickets);
        builder.Ignore(x => x.AnsweredTickets);
        builder.Ignore(x => x.ClosedTickets);

        builder.OwnsMany(x => x.Statuses, statusBuilder =>
        {
            statusBuilder.ToJson();

            statusBuilder.Property(x => x.TicketId)
                .HasMaxLength(50)
                .IsRequired();

            statusBuilder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();
        });
    }
}
