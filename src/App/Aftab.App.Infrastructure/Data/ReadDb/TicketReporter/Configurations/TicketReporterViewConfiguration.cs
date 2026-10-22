using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aftab.App.Infrastructure.Data.ReadDb.TicketReporter.Configurations;

internal sealed class TicketReporterViewConfiguration : IEntityTypeConfiguration<TicketReporterView>
{
    public void Configure(EntityTypeBuilder<TicketReporterView> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OrganId)
            .HasConversion<PrefixedGuidEfConverter<OrganId>>();
        
        builder.ComplexCollection(x => x.Statuses, repliesBuilder =>
        {
            repliesBuilder.ToJson(nameof(TicketReporterView.Statuses));
            repliesBuilder.Property(x => x.TicketId)
                .HasConversion<PrefixedGuidEfConverter<TicketId>>();
        });
    }
}
