using Aftab.Shared.Domain.Enums;
using Aftab.Shared.Domain.PrefixedGuidTools;

namespace Aftab.App.Domain.People.ValueObjects;

[Prefix(nameof(IdType.Idn))]
public record IdentityId(Guid Value) : PrefixedGuidV3(Value);
