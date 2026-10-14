namespace Aftab.Shared.Infrastructure.Security.ConfigureOptions.JwtOptions;

public class JwtOptions
{
    public const string SectionName = "JwtOptions";
    
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; }
}
