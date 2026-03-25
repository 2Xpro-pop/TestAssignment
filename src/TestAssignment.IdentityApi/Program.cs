using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Serialization;
using TestAssignment.IdentityApi.Infrastructure;
using TestAssignment.IdentityApi.V1;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddIdentityInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

var withApiVersioning = builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

var identity = app.NewVersionedApi("identity");

identity.MapIdentityApiV1();

app.Run();
