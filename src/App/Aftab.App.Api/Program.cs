using Aftab.App.Infrastructure;
using Aftab.App.Infrastructure.Data.Extensions;
using Aftab.App.Presentation;
using Aftab.App.Presentation.Ticketing.Policies.ReplyTicketOwnerShip;
using Aftab.App.Presentation.Ticketing.Policies.TicketOwnerShip;
using Aftab.Shared.Api;
using Aftab.Shared.Application;
using Aftab.Shared.Infrastructure;
using Microsoft.AspNetCore.Authorization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddApiConfiguration(
        assembliesToScanEndpoints: Aftab.App.Presentation.AssemblyReference.PresentationAssembly)
    .AddApplicationConfiguration(
        assembliesToScanRequestHandlers: Aftab.App.Application.AssemblyReference.ApplicationAssembly)
    .AddInfrastructureConfiguration(builder.Configuration)
    .AddAftabServices(
        writeDbConnString: "aftab-write-db",
        readDbConnString: "aftab-read-db",
        cacheConnString: "aftab-cache");

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AppPolices.Ticketing.CanManageTicket, policy =>
        policy.Requirements.Add(new TicketOwnershipRequirement(bypassRole: "Agent"))
    )
    .AddPolicy(AppPolices.Ticketing.CanManageReplyTicket, policy =>
        policy.Requirements.Add(new ReplyTicketOwnerShipRequirement(bypassRole: "Agent")));

builder.Services.AddScoped<IAuthorizationHandler, TicketOwnershipHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ReplyTicketOwnerShipHandler>();

WebApplication app = builder.Build();


await app.InitializeDatabaseAsync();
app.UseApiConfiguration();

await app.RunAsync();
