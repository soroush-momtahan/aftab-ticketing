using Aftab.Shared.Domain.Enums;
using Aftab.Shared.Domain.PrefixedGuidTools;

namespace Aftab.App.Domain.Ticketing.ValueObjects;

[Prefix(nameof(IdType.Tkt))]
public record TicketId(Guid Value) : PrefixedGuidV3(Value);
