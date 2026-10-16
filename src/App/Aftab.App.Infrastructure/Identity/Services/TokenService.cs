using System.Security.Claims;
using System.Text;
using Aftab.App.Application.Identity.Services;
using Aftab.Shared.Infrastructure.Security.ConfigureOptions.JwtOptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Aftab.App.Infrastructure.Identity.Services;

internal sealed class TokenService(
    IOptions<JwtOptions> jwtOptions,
    UserManager<IdentityUser> userManager) : ITokenService
{
    public async Task<string> GenerateTokenAsync(IdentityUser user)
    {
        JwtOptions options = jwtOptions.Value;

        // ۱. اضافه کردن Claim های پایه
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.PhoneNumber, user.PhoneNumber!),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        // ۲. اضافه کردن نقش‌های کاربر به توکن
        IList<string> roles = await userManager.GetRolesAsync(user);
        foreach (string role in roles)
        {
            // با JsonWebTokenHandler، این دقیقاً با نام "role" در توکن قرار می‌گیرد
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // ۳. ساخت کلید رمزنگاری
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // ۴. ساخت توصیف‌گر توکن (Token Descriptor)
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(options.ExpiryMinutes),
            Issuer = options.Issuer,
            Audience = options.Audience,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(tokenDescriptor);
    }
}
