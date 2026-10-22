using Aftab.App.Domain.Organization.Models;
using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.Shared.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aftab.App.Infrastructure.Organization.Configurations;

internal sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organ>
{
    public void Configure(EntityTypeBuilder<Organ> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion<PrefixedGuidEfConverter<OrganId>>();
    }
}
