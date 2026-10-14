using Aftab.App.Domain.Identity.Errors;
using Aftab.Shared.Application.Caching;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Identity.Services;

public class OtpService(
    ICacheService cacheService,
    IOtpGeneratorService otpGeneratorService,
    IOtpSenderService otpSenderService)
{
    public async Task<Result> SendOtpAsync(string mobile, CancellationToken cancellationToken)
    {
        string? code = await cacheService.GetAsync<string>($"Otp:{mobile}", cancellationToken);
        if (code is not null)
        {
            return Result.Success();
        }
        string otp = otpGeneratorService.GenerateMemorable6DigitOtp();
        try
        {
            await cacheService.SetAsync(
                $"Otp:{mobile}",
                otp,
                new TimeSpan(0, 0, 65),
                cancellationToken);
            await otpSenderService.SendOtpAsync(mobile, otp);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(IdentityErrors.SendOtpFailed);
        }
    }
}
