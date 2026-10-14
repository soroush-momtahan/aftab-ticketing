using Aftab.App.Domain.Identity.Enums;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.Shared.Domain.Objects;

namespace Aftab.App.Domain.People.Models;

public class Person : Aggregate<PersonId>
{
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 128;
    public const int MinPasswordUniqueChars = 4;
    
    public IdentityId IdentityId { get; private set; }
    public FullName FullName { get; private set; }
    public Mobile Mobile { get; private set; }
    public RoleType RoleType { get; private set; }
    
    private Person(){}

    private Person(
        IdentityId identityUserId,
        FullName fullName, 
        Mobile mobile)
    {
        IdentityId = identityUserId;
        FullName = fullName;
        Mobile = mobile;
    }

    public static Person Create(
        IdentityId identityUserId,
        FullName fullName, 
        Mobile mobile)
    {
        return new Person(
            identityUserId,
            fullName, 
            mobile);
    }
}
