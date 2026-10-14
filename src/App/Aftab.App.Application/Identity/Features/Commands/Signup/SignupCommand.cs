using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Identity.Features.Commands.Signup;

public record SignupCommand(
    string TempToken,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName) : ICommand<SignupCommand.Result>
{
    public record Result(string Token);
}
