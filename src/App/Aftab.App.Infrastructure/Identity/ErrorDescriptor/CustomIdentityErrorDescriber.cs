using Aftab.App.Domain.Identity.Errors;
using Aftab.App.Domain.People.Errors;
using Aftab.Shared.Domain.Errors;
using Microsoft.AspNetCore.Identity;

namespace Aftab.App.Infrastructure.Identity.ErrorDescriptor;

public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateUserName(string userName)
    {
        Error duplicateMobileError = PeopleErrors.DuplicateMobile(userName);
        return new IdentityError
        {
            Code = duplicateMobileError.Code,
            Description = duplicateMobileError.Description
        };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        Error passwordIsTooShortError = IdentityErrors.PasswordIsTooShort;
        return new IdentityError
        {
            Code = passwordIsTooShortError.Code,
            Description = passwordIsTooShortError.Description
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        Error passwordDoesNotContainNumber = IdentityErrors.PasswordDoesNotContainNumber;
        return new IdentityError
        {
            Code = passwordDoesNotContainNumber.Code,
            Description = passwordDoesNotContainNumber.Description
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        Error passwordDoesNotContainUppercaseLetter = IdentityErrors.PasswordDoesNotContainUppercaseLetter;
        return new IdentityError
        {
            Code = passwordDoesNotContainUppercaseLetter.Code,
            Description = passwordDoesNotContainUppercaseLetter.Description
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        Error passwordDoesNotContainLowercaseLetter = IdentityErrors.PasswordDoesNotContainLowercaseLetter;
        return new IdentityError
        {
            Code = passwordDoesNotContainLowercaseLetter.Code,
            Description = passwordDoesNotContainLowercaseLetter.Description
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        Error passwordDoesNotContainSpecialLetter = IdentityErrors.PasswordDoesNotContainSpecialLetter;
        return new IdentityError
        {
            Code = passwordDoesNotContainSpecialLetter.Code,
            Description = passwordDoesNotContainSpecialLetter.Description
        };
    }

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        Error passwordRequiresUniqueChars = IdentityErrors.PasswordRequiresUniqueChars;
        return new IdentityError
        {
            Code = passwordRequiresUniqueChars.Code,
            Description = passwordRequiresUniqueChars.Description
        };
    }
}
