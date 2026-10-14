using Aftab.Shared.Domain.Enums;
using Aftab.Shared.Domain.PrefixedGuidTools;

namespace Aftab.App.Domain.Ticketing.ValueObjects;

[Prefix(nameof(IdType.RplTkt))]
public record ReplyTicketId(Guid Value) : PrefixedGuidV3(Value);
