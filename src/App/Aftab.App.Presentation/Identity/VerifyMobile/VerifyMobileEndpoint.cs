using Aftab.App.Application.Identity.Features.Commands.VerifyMobile;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Identity.VerifyMobile;

public sealed class VerifyMobileEndpoint : IEndpoint
{
    public sealed record VerifyMobileRequest(
        string Code,
        string Mobile)
    {
        public sealed record Response(
            bool IsNewUser,
            string TempToken);
    }
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/mobile/{mobile}/verify/{otpCode}", async (
            string mobile, string otpCode, ISender sender) =>
        {
            Result<VerifyMobileResult> result = await sender.Send(
                ToCommand(new VerifyMobileRequest(otpCode, mobile)));
            
            Result<VerifyMobileRequest.Response> response = ToResponse(result);
            
            return response.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithTags(Tags.Identity);
    }

    private static VerifyMobileCommand ToCommand(VerifyMobileRequest request)
    {
        return new VerifyMobileCommand(request.Code, request.Mobile);
    }

    private static Result<VerifyMobileRequest.Response> ToResponse(Result<VerifyMobileResult> result)
    {
        if (result.IsFailure)
        {
            return Result.Failure<VerifyMobileRequest.Response>(result.Error);
        }

        return new VerifyMobileRequest.Response(result.Value.IsNewUser, result.Value.Token);
    }
}
