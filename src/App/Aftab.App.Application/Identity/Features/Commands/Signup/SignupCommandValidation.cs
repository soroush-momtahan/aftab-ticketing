using Aftab.App.Application.Identity.Extensions;
using Aftab.App.Domain.Identity.Errors;
using Aftab.App.Domain.People.Errors;
using Aftab.App.Domain.People.ValueObjects;
using FluentValidation;

namespace Aftab.App.Application.Identity.Features.Commands.Signup;

internal sealed class SignupCommandValidation : AbstractValidator<SignupCommand>
{
    public SignupCommandValidation()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(PeopleErrors.FirstnameIsEmpty.Description)
            .MinimumLength(FullName.MinLenght)
            .WithMessage(PeopleErrors.FirstnameIsTooShort.Description)
            .MaximumLength(FullName.MaxLenght)
            .WithMessage(PeopleErrors.FirstnameIsTooLong.Description);
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(PeopleErrors.LastnameIsEmpty.Description)
            .MinimumLength(FullName.MinLenght)
            .WithMessage(PeopleErrors.LastnameIsTooShort.Description)
            .MaximumLength(FullName.MaxLenght)
            .WithMessage(PeopleErrors.LastnameIsTooLong.Description);

        RuleFor(x => x.Password)
            .PasswordRules();
        
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
        .WithMessage(IdentityErrors.ConfirmPasswordIsNotMatch.Description);
    }
}
