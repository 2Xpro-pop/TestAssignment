using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

if (builder.Environment.IsProduction())
{
    postgres.WithPassword(builder.AddParameter("postgres-password", secret: true));
}

var identityDb = postgres.AddDatabase("identitydb");
var paymentDb = postgres.AddDatabase("paymentdb");

builder.AddProject<Projects.TestAssignment_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.TestAssignment_IdentityApi>("testassignment-identityapi")
    .WithReference(identityDb).WaitFor(identityDb)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.TestAssignment_PaymentApi>("testassignment-paymentapi")
    .WithReference(paymentDb)
    .WaitFor(paymentDb)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
