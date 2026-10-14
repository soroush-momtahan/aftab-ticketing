using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Identity.Features.Commands.SendOtp;

public record SendOtpCommand(string Mobile) : ICommand;
