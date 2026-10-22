using Aftab.App.Infrastructure;
using Aftab.App.Infrastructure.Data;
using Aftab.Shared.Application;
using Aftab.Shared.Infrastructure.Extensions;
using Aftab.Shared.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection.Extensions;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);



builder
    .AddApplicationConfiguration(
        assembliesToScanRequestHandlers: Aftab.App.Application.AssemblyReference.ApplicationAssembly)
    .AddOutboxWorkerConfiguration<AftabDbContext>(
        moduleName: "OutboxWorker",
        assemblyToScan: Aftab.App.Application.AssemblyReference.ApplicationAssembly)
    .AddAftabServices(
        writeDbConnString: "aftab-write-db", 
        readDbConnString: "aftab-read-db",
        cacheConnString: "aftab-cache-db");

builder.Services.TryAddSingleton<InsertOutboxMessageInterceptor>();

IHost host = builder.Build();

await host.RunAsync();
