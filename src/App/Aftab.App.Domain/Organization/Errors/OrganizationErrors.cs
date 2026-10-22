using Aftab.Shared.Domain.Errors;

namespace Aftab.App.Domain.Organization.Errors;

public static class OrganizationErrors
{
    public static readonly Error NotFound = new(
        "Organ.NotFound",
        "سازمان مورد نظر یافت نشد",
        ErrorType.NotFound);
}
