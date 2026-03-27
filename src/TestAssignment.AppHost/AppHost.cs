using Microsoft.Extensions.Hosting;
using TestAssignment.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

if (builder.Environment.IsProduction())
{
    postgres.WithPassword(builder.AddParameter("postgres-password", secret: true));
}

postgres.WithPgAdmin();

var yarp = builder.AddYarp("yarp");

var identityDb = postgres.AddDatabase("identitydb");
var paymentDb = postgres.AddDatabase("paymentdb");

var identityApi = builder.AddProject<Projects.TestAssignment_IdentityApi>("testassignment-identityapi")
    .WithReference(identityDb).WaitFor(identityDb)
    .WithHttpHealthCheck("/health");

var paymentApi = builder.AddProject<Projects.TestAssignment_PaymentApi>("testassignment-paymentapi")
    .WithReference(paymentDb)
    .WaitFor(paymentDb)
    .WithReference(identityApi)
    .WaitFor(identityApi)
    .WithHttpHealthCheck("/health");

var webfrontend = builder.AddProject<Projects.TestAssignment_Web>("webfrontend")
    .WithReference(yarp)
    .WithHttpHealthCheck("/health");

yarp.ConfigureTestAssignmentRoutes(identityApi: identityApi, paymentApi: paymentApi, webFrontend: webfrontend)
    .WithExternalHttpEndpoints();

builder.Build().Run();
