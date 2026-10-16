using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetTicketOwnerId;

public record GetTicketOwnerIdQuery(
    string TicketId) : IQuery<GetTicketOwnerIdQuery.Result>
{
    public record Result(string OwnerId);
}
