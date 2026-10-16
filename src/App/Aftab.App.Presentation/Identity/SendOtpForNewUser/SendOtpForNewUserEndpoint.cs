using Aftab.App.Application.Identity.Features.Commands.SendOtpForNewUser;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Identity.SendOtpForNewUser;

public sealed class SendOtpForNewUserEndpoint : IEndpoint
{
    public sealed record SendOtpForNewUserRequest(
        string Mobile)
    {
        public sealed record Response(bool IsExistingUser); 
    }
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/newUser/sendOtp", async (SendOtpForNewUserRequest request, ISender sender) =>
        {
            Result<SendOtpForNewUserResult> result = await sender.Send(ToCommand(request));
            Result<SendOtpForNewUserRequest.Response> response = ToResponse(result);
            return response.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithTags(Tags.Identity);
    }

    private static SendOtpForNewUserCommand ToCommand(SendOtpForNewUserRequest request)
    {
        return new SendOtpForNewUserCommand(request.Mobile);
    }

    private static Result<SendOtpForNewUserRequest.Response> ToResponse(Result<SendOtpForNewUserResult> result)
    {
        if (result.IsFailure)
        {
            return Result.Failure<SendOtpForNewUserRequest.Response>(result.Error);
        }
        return new SendOtpForNewUserRequest.Response(result.Value.IsUserExist);
    }
}
