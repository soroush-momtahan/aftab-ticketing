using Aftab.App.Application.Identity.Services;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Identity.Features.Commands.SendOtp;

public sealed class SendOtpCommandHandler(OtpService otpService) 
    : ICommandHandler<SendOtpCommand>
{
    public async Task<Result> Handle(SendOtpCommand command, CancellationToken cancellationToken)
    {
        return await otpService.SendOtpAsync(command.Mobile, cancellationToken);
    }
}
