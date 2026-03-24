using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

if (builder.Environment.IsProduction())
{
    postgres.WithPassword(builder.AddParameter("postgres-password", secret: true));
}

var identityDb = postgres.AddDatabase("identitydb");

var apiService = builder.AddProject<Projects.TestAssignment_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.TestAssignment_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddProject<Projects.TestAssignment_IdentityApi>("testassignment-identityapi")
    .WithReference(identityDb).WaitFor(identityDb);

builder.Build().Run();
