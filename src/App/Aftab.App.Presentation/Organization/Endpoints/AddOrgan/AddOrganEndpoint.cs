using Aftab.App.Application.Organization.Features.Commands.AddOrgan;
using Aftab.Shared.Application.Authorization;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Organization.Endpoints.AddOrgan;

internal sealed class AddOrganEndpoint : IEndpoint
{
    public sealed record AddOrganRequest(
        string Name,
        string Description);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/organs", async (
            ISender sender, AddOrganRequest request) =>
        {
            Result result = await sender.Send(
                new AddOrganCommand(request.Name, request.Description));
            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Organization)
        .RequireAuthorization(AppPermissions.Organs.Create);
    }
}
