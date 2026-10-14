using Aftab.App.Domain.People.Errors;
using FluentValidation;

namespace Aftab.App.Application.Identity.Features.Commands.SendOtp;

public sealed class SendOtpCommandValidation : AbstractValidator<SendOtpCommand>
{
    public SendOtpCommandValidation()
    {
        RuleFor(x => x.Mobile)
            .NotEmpty()
            .WithMessage(PeopleErrors.MobileNotValid.Description)
            .Length(11)
            .WithMessage(PeopleErrors.MobileNotValid.Description);
    }
}
