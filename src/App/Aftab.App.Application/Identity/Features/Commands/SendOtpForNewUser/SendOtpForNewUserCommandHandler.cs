using Aftab.App.Application.Identity.Services;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.Repository;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Identity.Features.Commands.SendOtpForNewUser;

internal sealed class SendOtpForNewUserCommandHandler(
    IPeopleRepository peopleRepository,
    OtpService otpService) : ICommandHandler<SendOtpForNewUserCommand, SendOtpForNewUserResult>
{
    public async Task<Result<SendOtpForNewUserResult>> Handle(SendOtpForNewUserCommand command, CancellationToken cancellationToken)
    {
        Person? user = await peopleRepository.FindByMobileAsync(command.Mobile, cancellationToken);
        if (user is not null)
        {
            return new SendOtpForNewUserResult(true);
        }

        Result sendOtpOrError = await otpService.SendOtpAsync(command.Mobile, cancellationToken);
        if (sendOtpOrError.IsFailure)
        {
            return Result.Failure<SendOtpForNewUserResult>(sendOtpOrError.Error);
        }
        
        return new SendOtpForNewUserResult(false);
    }
}
