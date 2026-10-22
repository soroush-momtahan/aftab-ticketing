using Aftab.App.Domain.Identity.Enums;
using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.App.Domain.People.Events;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.Shared.Domain.Objects;

namespace Aftab.App.Domain.People.Models;

public class Person : Aggregate<PersonId>
{
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 128;
    public const int MinPasswordUniqueChars = 4;
    
    public FullName FullName { get; private set; }
    public Mobile Mobile { get; private set; }
    public RoleType RoleType { get; private set; }
    public OrganId? OrganId { get; private set; }
    
    private Person(){}

    private Person(
        FullName fullName, 
        Mobile mobile)
    {
        FullName = fullName;
        Mobile = mobile;
    }

    public static Person Create(
        FullName fullName, 
        Mobile mobile)
    {
        return new Person(
            fullName, 
            mobile);
    }
    public void AssignToOrgan(OrganId organId)
    {
        OrganId = organId;
        RoleType = RoleType.Agent;
        AddDomainEvent(new PersonAssignedToOrganEvent(Id));
    }
}
