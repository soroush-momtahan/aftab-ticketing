namespace Aftab.App.Application.Identity.Services;

public interface IOtpGeneratorService
{
    string GenerateSecureOtp();
    string GenerateMemorableOtp();
    string GenerateMemorable6DigitOtp();
}
