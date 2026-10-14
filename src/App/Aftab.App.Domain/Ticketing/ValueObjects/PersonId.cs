using Aftab.Shared.Domain.Enums;
using Aftab.Shared.Domain.PrefixedGuidTools;

namespace Aftab.App.Domain.Ticketing.ValueObjects;

[Prefix(nameof(IdType.Prn))]
public record PersonId(Guid Value) : PrefixedGuidV3(Value);
