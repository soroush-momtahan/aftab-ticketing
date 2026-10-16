using System.Security.Claims;
using Aftab.App.Application.Ticketing.Features.Queries.GetTicket;
using Aftab.App.Domain.Ticketing.Enums;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Ticketing.Endpoints.GetTicket;

internal sealed class GetTicketEndpoint : IEndpoint
{
    public sealed record GetTicketRequest
    {
        public sealed record Response(
            TicketDto TicketDto,
            List<ReplyTicketDto> Replies);

        public sealed record ReplyTicketDto(
            string Id,
            string ReplierId,
            string ReplierDisplayName,
            string Content);

        public sealed record TicketDto(
            string Id,
            string PersonId,
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

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Tickets/{ticketId}", async (
                string ticketId,
                ClaimsPrincipal person,
                ISender sender) =>
            {
                Result<GetTicketQuery.Result> result = await sender.Send(new GetTicketQuery(ticketId));
                Result<GetTicketRequest.Response> response = ToResponse(result);
                return response.Match(Results.Ok, ApiResults.Problem);
            })
            .WithTags(Tags.Ticketing)
            .RequireAuthorization(AppPolices.Ticketing.CanManageTicket);
    }
    
    private static Result<GetTicketRequest.Response> ToResponse(Result<GetTicketQuery.Result> result)
    {
        if (result.IsFailure)
        {
            return Result.Failure<GetTicketRequest.Response>(result.Error);
        }

        var ticketDto = new GetTicketRequest.TicketDto(
            result.Value.TicketDto.Id,
            result.Value.TicketDto.PersonId,
            result.Value.TicketDto.DisplayName,
            result.Value.TicketDto.Title,
            result.Value.TicketDto.Content,
            result.Value.TicketDto.Priority,
            result.Value.TicketDto.Status,
            result.Value.TicketDto.Department,
            result.Value.TicketDto.CreatedAt,
            result.Value.TicketDto.ModifiedAt,
            result.Value.TicketDto.DeletedAt);

        var replyTicketDto = result.Value.Replies.Select(ToReplyDto).ToList();

        return new GetTicketRequest.Response(
            ticketDto,
            replyTicketDto);
    }

    private static GetTicketRequest.ReplyTicketDto ToReplyDto(GetTicketQuery.ReplyTicketDto replyTicketDto)
    {
        return new GetTicketRequest.ReplyTicketDto(
            replyTicketDto.Id,
            replyTicketDto.ReplierId,
            replyTicketDto.ReplierDisplayName,
            replyTicketDto.Content);
    }
}
