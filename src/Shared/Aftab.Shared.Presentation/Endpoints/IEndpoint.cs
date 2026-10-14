using Microsoft.AspNetCore.Routing;

namespace Aftab.Shared.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
