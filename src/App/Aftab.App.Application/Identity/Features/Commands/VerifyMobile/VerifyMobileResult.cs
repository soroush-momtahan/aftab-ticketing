namespace Aftab.App.Application.Identity.Features.Commands.VerifyMobile;

public record VerifyMobileResult(
    bool IsNewUser,
    string Token);
