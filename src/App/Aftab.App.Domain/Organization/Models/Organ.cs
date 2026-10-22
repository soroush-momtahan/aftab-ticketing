using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.Shared.Domain.Objects;

namespace Aftab.App.Domain.Organization.Models;

public class Organ : Aggregate<OrganId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    private Organ() {}

    public static Organ Create(
        string name,
        string description)
    {
        return new Organ
        {
            Name = name,
            Description = description
        };
    }
}
