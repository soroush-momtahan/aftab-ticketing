using Aftab.App.Domain.Identity.Errors;
using Aftab.App.Domain.People.Errors;
using FluentValidation;

namespace Aftab.App.Application.Identity.Features.Commands.VerifyMobile;

internal sealed class VerifyMobileCommandValidation : AbstractValidator<VerifyMobileCommand>
{
    public VerifyMobileCommandValidation()
    {
        RuleFor(x => x.Mobile)
            .NotEmpty()
            .WithMessage(PeopleErrors.MobileNotValid.Description)
            .Length(11)
            .WithMessage(PeopleErrors.MobileNotValid.Description);

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(IdentityErrors.VerifyCodeIsEmpty.Description);
    }
}
