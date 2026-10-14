using Aftab.App.Domain.People.Errors;
using FluentValidation;

namespace Aftab.App.Application.Identity.Features.Commands.SendOtpForNewUser;

internal sealed class SendOtpForNewUserValidation : AbstractValidator<SendOtpForNewUserCommand>
{
    public SendOtpForNewUserValidation()
    {
        RuleFor(x => x.Mobile)
            .NotEmpty()
            .WithMessage(PeopleErrors.MobileNotValid.Description)
            .Length(11)
            .WithMessage(PeopleErrors.MobileNotValidLenght.Description);
    }
}
