using Aftab.App.Application.Identity.Features.Commands.Signup;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Identity.SignUp;

public sealed class SignUpEndpoint : IEndpoint
{
    public sealed record SignUpRequest(
        string TempToken,
        string Password,
        string ConfirmPassword,
        string FirstName,
        string LastName)
    {
        public sealed record Response(string Token);
    }
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/signUp", async (SignUpRequest request, ISender sender) =>
        {
            Result<SignupCommand.Result> result = await sender.Send(ToCommand(request));
            Result<SignUpRequest.Response> response = ToResponse(result);
            return response.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithTags(Tags.Identity);
    }

    private static SignupCommand ToCommand(SignUpRequest request)
    {
        return new SignupCommand(
            request.TempToken, 
            request.Password, 
            request.ConfirmPassword, 
            request.FirstName, 
            request.LastName);
    }

    private static Result<SignUpRequest.Response> ToResponse(Result<SignupCommand.Result> result)
    {
        if (result.IsFailure)
        {
            return Result.Failure<SignUpRequest.Response>(result.Error);
        }
        return new SignUpRequest.Response(result.Value.Token);
    }
}
