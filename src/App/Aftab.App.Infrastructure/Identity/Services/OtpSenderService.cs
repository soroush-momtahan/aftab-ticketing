using Aftab.App.Application.Identity.Services;

namespace Aftab.App.Infrastructure.Identity.Services;

internal sealed class OtpSenderService : IOtpSenderService
{
    public Task SendOtpAsync(string mobile, string code)
    {
        Console.WriteLine(code);
        return Task.CompletedTask;
    }
}
