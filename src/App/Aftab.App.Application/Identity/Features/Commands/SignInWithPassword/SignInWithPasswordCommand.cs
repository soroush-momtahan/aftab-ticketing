using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Identity.Features.Commands.SignInWithPassword;

public record SignInWithPasswordCommand(
    string Mobile,
    string Password) : ICommand<SignInWithPasswordCommand.Result>
{
    public record Result(string Token);
};
