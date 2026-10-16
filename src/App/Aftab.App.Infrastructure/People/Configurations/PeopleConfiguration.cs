using Aftab.App.Domain.Identity.Enums;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.Shared.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aftab.App.Infrastructure.People.Configurations;

internal sealed class PeopleConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion<PrefixedGuidEfConverter<PersonId>>();

        builder.ComplexProperty(x => x.FullName, fullNameBuilder =>
        {
            fullNameBuilder.Property(x => x.Firstname)
                .HasColumnName(nameof(Person.FullName));
            
            fullNameBuilder.Property(x => x.Firstname)
                .HasColumnName(nameof(Person.FullName.Lastname));
        });

        builder.ComplexProperty(x => x.Mobile, mobileBuilder =>
        {
            mobileBuilder.Property(x => x.Value)
                .HasColumnName(nameof(Mobile));
        });

        builder.Property(x => x.RoleType)
            .HasConversion<string>()
            .HasDefaultValue(RoleType.Client);
    }
}
