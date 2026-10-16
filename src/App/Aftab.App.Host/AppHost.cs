using Projects;

#pragma warning disable S125
// using Projects;
#pragma warning restore S125

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> sharedPostgresServer =
    builder
        .AddPostgres("postgres")
        .WithDataVolume() 
        .WithLifetime(ContainerLifetime.Persistent)
        .WithPgAdmin(resourceBuilder => { resourceBuilder.WithLifetime(ContainerLifetime.Persistent); });

IResourceBuilder<PostgresDatabaseResource> aftabWriteDb = sharedPostgresServer.AddDatabase("aftab-write-db");
IResourceBuilder<PostgresDatabaseResource> aftabReadDb = sharedPostgresServer.AddDatabase("aftab-Read-db");

IResourceBuilder<RedisResource> aftabCache = builder.AddRedis("aftab-cache")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithRedisInsight();



IResourceBuilder<ProjectResource> aftabApi = builder.AddProject<Aftab_App_Api>("aftab-api")
    .WithReference(aftabWriteDb)
    .WaitFor(aftabWriteDb)
    .WithReference(aftabReadDb)
    .WaitFor(aftabReadDb)
    .WithReference(aftabCache)
    .WaitFor(aftabCache)
    .WithExternalHttpEndpoints();

builder.AddProject<Aftab_App_OutboxWorker>("aftab-outbox-worker")
    .WithReference(aftabWriteDb)
    .WaitFor(aftabWriteDb)
    .WithReference(aftabReadDb)
    .WaitFor(aftabReadDb)
    .WaitFor(aftabApi);

await builder.Build().RunAsync();
