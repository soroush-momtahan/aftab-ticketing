namespace Aftab.Shared.Api.ConfigureOptions.ApiInfoOptions;

public class ApiInfoOptions
{
    public const string SectionName = "ApiInfo";

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = "v1";
    public string? TermsOfServiceUrl { get; set; }
    public ContactInfo? Contact { get; set; }
    public LicenseInfo? License { get; set; }
}

public class ContactInfo
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
}

public class LicenseInfo
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
}
