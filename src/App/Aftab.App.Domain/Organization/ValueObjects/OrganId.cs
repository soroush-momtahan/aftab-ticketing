using Aftab.Shared.Domain.Enums;
using Aftab.Shared.Domain.PrefixedGuidTools;

namespace Aftab.App.Domain.Organization.ValueObjects;

[Prefix(nameof(IdType.Org))]
public record OrganId(Guid Value) : PrefixedGuidV3(Value);
