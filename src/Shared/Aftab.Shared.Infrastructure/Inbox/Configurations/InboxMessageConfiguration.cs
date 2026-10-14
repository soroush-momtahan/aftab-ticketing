using Aftab.Shared.Infrastructure.Inbox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aftab.Shared.Infrastructure.Inbox.Configurations;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>  
{  
    public void Configure(EntityTypeBuilder<InboxMessage> builder)  
    {        
        builder.ToTable("InboxMessages");
    
        builder.HasKey(o => o.Id);  
          
        builder.Property(o => o.Content).HasMaxLength(2000);
    }
}
