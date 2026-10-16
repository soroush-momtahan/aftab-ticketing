using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetReplyTicketOwnerId;

public record GetReplyTicketOwnerIdQuery(
    string ReplyTicketId) : IQuery<GetReplyTicketOwnerIdQuery.Result>
{
    public record Result(string OwnerId);
}
