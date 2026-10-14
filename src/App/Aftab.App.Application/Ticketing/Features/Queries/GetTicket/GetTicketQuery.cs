using Aftab.App.Application.Ticketing.Services;
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
    
    public static Result ToResult(TicketView ticketView)
    {
        
        var ticketDto = new TicketDto(
            ticketView.Id,
            ticketView.DisplayName,
            ticketView.Title,
            ticketView.Content,
            ticketView.Priority,
            ticketView.Status,
            ticketView.Department,
            ticketView.CreatedAt,
            ticketView.ModifiedAt,
            ticketView.DeletedAt);

        var replyDtos = ticketView.Replies.Select(ToReplyDto).ToList();

        return new GetTicketQuery.Result(
            ticketDto,
            replyDtos);
    }

    private static ReplyTicketDto ToReplyDto(TicketView.ReplyTicketView replyTicketView)
    {
        return new ReplyTicketDto(
            replyTicketView.Id,
            replyTicketView.ReplierId,
            replyTicketView.ReplierDisplayName,
            replyTicketView.Content);
    }
}
