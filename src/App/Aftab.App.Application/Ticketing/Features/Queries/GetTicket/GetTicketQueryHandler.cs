using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetTicket;

internal sealed class GetTicketQueryHandler(
    ITicketingViewService ticketingViewService) : ICommandHandler<GetTicketQuery, GetTicketQuery.Result>
{
    public async Task<Result<GetTicketQuery.Result>> Handle(
        GetTicketQuery request, CancellationToken cancellationToken)
    {
        Result<TicketId> ticketIdOrError = PrefixedGuidV3.From<TicketId>(request.TicketId);
        if (ticketIdOrError.IsFailure)
        {
            return Result.Failure<GetTicketQuery.Result>(ticketIdOrError.Error);
        }
        
        TicketView? ticketView = await ticketingViewService.GetByIdAsync(ticketIdOrError.Value, cancellationToken);
        if (ticketView is null)
        {
            return Result.Failure<GetTicketQuery.Result>(TicketingErrors.NotFound(request.TicketId));
        }

        return ToResult(ticketView);
    }

    private static GetTicketQuery.Result ToResult(TicketView ticketView)
    {
        
        var ticketDto = new GetTicketQuery.TicketDto(
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

    private static GetTicketQuery.ReplyTicketDto ToReplyDto(TicketView.ReplyTicketView replyTicketView)
    {
        return new GetTicketQuery.ReplyTicketDto(
            replyTicketView.Id,
            replyTicketView.ReplierId,
            replyTicketView.ReplierDisplayName,
            replyTicketView.Content);
    }
}
