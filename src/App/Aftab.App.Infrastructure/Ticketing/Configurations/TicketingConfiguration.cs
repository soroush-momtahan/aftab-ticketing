using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aftab.App.Infrastructure.Ticketing.Configurations;

internal sealed class TicketingConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion<PrefixedGuidEfConverter<TicketId>>();
        
        builder.Property(x => x.PersonId)
            .HasConversion<PrefixedGuidEfConverter<PersonId>>();
        
        builder.Property(x => x.OrganId)
            .HasConversion<PrefixedGuidEfConverter<OrganId>>()
            .IsRequired(false);

        builder.ComplexProperty(x => x.Title, titleBuilder =>
        {
            titleBuilder.Property(x => x.Value)
                .HasColumnName(nameof(Ticket.Title));
        });

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.Property(x => x.Priority)
            .HasConversion<string>();

        builder.Property(x => x.Department)
            .HasConversion<string>();

        builder.OwnsMany(x => x.ReplyTickets, replyTicketsBuilder =>
        {
            replyTicketsBuilder.WithOwner()
                .HasForeignKey(nameof(TicketId));
            
            replyTicketsBuilder.Property(x => x.Id)
                .HasConversion<PrefixedGuidEfConverter<ReplyTicketId>>();
            
            replyTicketsBuilder.Property(x => x.TicketId)
                .HasConversion<PrefixedGuidEfConverter<TicketId>>();
            
            replyTicketsBuilder.Property(x => x.ReplierId)
                .HasConversion<PrefixedGuidEfConverter<PersonId>>();
        });
    }
}
