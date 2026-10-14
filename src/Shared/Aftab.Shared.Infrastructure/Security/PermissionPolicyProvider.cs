using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Aftab.Shared.Infrastructure.Security;

public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // ابتدا چک می‌کنیم آیا این یک پالیسی پیش‌فرض دات‌نت است؟
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);
        if (policy is not null)
        {
            return policy;
        }

        // اگر نبود، فرض می‌کنیم یک نام پرمیشن است (مثل catalog:create) و آن را داینامیک می‌سازیم
        return new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
    }
}
