using Aftab.App.Domain.Ticketing.Enums;
using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetTicket;

public record GetTicketQuery(
    string TicketId) : ICommand<GetTicketQuery.Result>
{
    public record Result(
        TicketDto TicketDto,
        List<ReplyTicketDto> Replies);

    public record ReplyTicketDto(
        string Id,
        string ReplierId,
        string ReplierDisplayName,
        string Content);

    public record TicketDto(
        string Id,
        string DisplayName,
        string Title,
        string Content,
        TicketPriority Priority,
        TicketStatus Status,
        TicketDepartment Department,
        DateTime CreatedAt,
        DateTime? ModifiedAt,
        DateTime? DeletedAt);
}
