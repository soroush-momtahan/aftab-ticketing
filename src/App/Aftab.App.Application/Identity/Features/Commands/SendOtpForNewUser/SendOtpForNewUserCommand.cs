using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Identity.Features.Commands.SendOtpForNewUser;

public record SendOtpForNewUserCommand(
    string Mobile) : ICommand<SendOtpForNewUserResult>;
