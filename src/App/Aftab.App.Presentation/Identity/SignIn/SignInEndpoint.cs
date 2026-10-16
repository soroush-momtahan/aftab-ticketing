using Aftab.App.Application.Identity.Features.Commands.SignInWithPassword;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Identity.SignIn;

public sealed class SignInEndpoint : IEndpoint
{
    public sealed record SignInRequest(
        string Mobile,
        string Password)
    {
        public sealed record Response(string Token); 
    };
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/signIn", async (SignInRequest request, ISender sender) =>
        {
            Result<SignInWithPasswordCommand.Result> result = await sender.Send(ToCommand(request));
            Result<SignInRequest.Response> response = ToResponse(result);
            return response.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Identity);
    }

    private static SignInWithPasswordCommand ToCommand(SignInRequest request)
    {
        return new SignInWithPasswordCommand(request.Mobile, request.Password);
    }

    private static Result<SignInRequest.Response> ToResponse(Result<SignInWithPasswordCommand.Result> result)
    {
        if (result.IsFailure)
        {
            return Result.Failure<SignInRequest.Response>(result.Error);
        }
        return new SignInRequest.Response(result.Value.Token);
    }
}
