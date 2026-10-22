using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.App.Infrastructure.Data.ReadDb.TicketReporter.Configurations;
using Aftab.Shared.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aftab.App.Infrastructure.Data.ReadDb;

public sealed class AftabReadDbContext(DbContextOptions<AftabReadDbContext> options) 
    : DbContext(options)
{
    public DbSet<TicketView> TicketViews => Set<TicketView>();
    public DbSet<TicketReporterView> TicketReporterViews => Set<TicketReporterView>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new TicketReporterViewConfiguration());
        EntityTypeBuilder<TicketView> ticketViewBuilder = modelBuilder.Entity<TicketView>();
        
        ticketViewBuilder.HasKey(ticketView => ticketView.Id);
        ticketViewBuilder
            .Property(x => x.Id)
            .HasConversion<PrefixedGuidEfConverter<TicketId>>();

        ticketViewBuilder.Property(x => x.OrganId)
            .HasConversion<PrefixedGuidEfConverter<OrganId>>()
            .IsRequired(false);
        
        ticketViewBuilder
            .Property(x => x.PersonId)
            .HasConversion<PrefixedGuidEfConverter<PersonId>>();

        ticketViewBuilder.Property(x => x.Priority)
            .HasConversion<string>();
        
        ticketViewBuilder.Property(x => x.Status)
            .HasConversion<string>();
        
        ticketViewBuilder.Property(x => x.Department)
            .HasConversion<string>();
        
        ticketViewBuilder.ComplexCollection(x => x.Replies, repliesBuilder =>
        {
            repliesBuilder.ToJson(nameof(TicketView.Replies));
            repliesBuilder.Property(x => x.Id)
                .HasConversion<PrefixedGuidEfConverter<ReplyTicketId>>();
            repliesBuilder.Property(x => x.ReplierId)
                .HasConversion<PrefixedGuidEfConverter<PersonId>>();
        });
    }
}
