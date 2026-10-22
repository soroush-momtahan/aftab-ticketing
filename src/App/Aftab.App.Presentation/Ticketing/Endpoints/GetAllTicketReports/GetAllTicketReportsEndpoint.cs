using Aftab.App.Application.Ticketing.Features.Queries.GetAllTicketReports;
using Aftab.Shared.Application.Authorization;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Ticketing.Endpoints.GetAllTicketReports;

internal sealed class GetAllTicketReportsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets/reports", async (
            ISender sender) =>
        {
            Result<GetAllTicketReportsQuery.Result> result = await sender.Send(new GetAllTicketReportsQuery());
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Ticketing)
        .RequireAuthorization(AppPermissions.Tickets.Reports);
    }
}
