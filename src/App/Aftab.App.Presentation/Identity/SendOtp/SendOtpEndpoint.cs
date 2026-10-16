using Aftab.App.Application.Identity.Features.Commands.SendOtp;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Identity.SendOtp;

internal sealed class SendOtpEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/otp/{mobile}", async (string mobile, ISender sender) =>
        {
            Result result = await sender.Send(new SendOtpCommand(mobile));
            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Identity);
    }
}
