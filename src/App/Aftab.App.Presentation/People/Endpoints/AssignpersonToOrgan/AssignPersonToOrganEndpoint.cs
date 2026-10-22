using Aftab.App.Application.People.Commands.AssignPersonToOrgan;
using Aftab.Shared.Application.Authorization;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.People.Endpoints.AssignpersonToOrgan;

internal sealed class AssignPersonToOrganEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/people/{personId}/organs/{organId}", async (
            string personId,
            string organId,
            ISender sender) =>
        {
            Result result = await sender.Send(
                new AssignPersonToOrganCommand(personId, organId));
            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.People)
        .RequireAuthorization(AppPermissions.Tickets.Update);
    }
}
