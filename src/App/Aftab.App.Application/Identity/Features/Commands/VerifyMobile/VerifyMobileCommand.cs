using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Identity.Features.Commands.VerifyMobile;

public record VerifyMobileCommand(
    string Code,
    string Mobile) : ICommand<VerifyMobileResult>;
