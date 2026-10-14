using Aftab.Shared.Infrastructure.Inbox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aftab.Shared.Infrastructure.Inbox.Configurations;

public sealed class InboxMessageConsumerConfiguration : IEntityTypeConfiguration<InboxMessageConsumer>  
{  
    public void Configure(EntityTypeBuilder<InboxMessageConsumer> builder) 
    {        builder.ToTable("InboxMessageConsumers");
  
        builder.HasKey(o => new { o.InboxMessageId, o.Name });
  
        builder.Property(o => o.Name).HasMaxLength(500);
    }
}
