using Aftab.App.Domain.Identity.Errors;
using Aftab.App.Domain.People.Models;
using FluentValidation;

namespace Aftab.App.Application.Identity.Extensions;

public static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> PasswordRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(Person.MinPasswordLength)
            .WithMessage(IdentityErrors.PasswordIsTooShort.Description)

            .MaximumLength(Person.MaxPasswordLength)
            .WithMessage(IdentityErrors.PasswordIsTooLong.Description) // جلوگیری از هش‌های طولانی (DoS)

            .Matches("[A-Z]")
            .WithMessage(IdentityErrors.PasswordDoesNotContainUppercaseLetter.Description)

            .Matches("[a-z]")
            .WithMessage(IdentityErrors.PasswordDoesNotContainLowercaseLetter.Description)

            .Matches("[0-9]")
            .WithMessage(IdentityErrors.PasswordDoesNotContainNumber.Description)

            .Matches("[^a-zA-Z0-9]")
            .WithMessage(IdentityErrors.PasswordDoesNotContainSpecialLetter.Description)

            .MinimumUniqueChars(Person.MinPasswordUniqueChars);
    }
    private static IRuleBuilderOptions<T, string> MinimumUniqueChars<T>(
        this IRuleBuilder<T, string> ruleBuilder, int requiredUniqueChars)
    {
        return ruleBuilder.Must(password => 
                !string.IsNullOrEmpty(password) && password.Distinct().Count() >= requiredUniqueChars)
            .WithMessage(IdentityErrors.PasswordRequiresUniqueChars.Description);
    }
}
