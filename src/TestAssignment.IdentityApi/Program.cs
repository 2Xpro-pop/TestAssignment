using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using TestAssignment.IdentityApi.Infrastructure;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();

builder.AddIdentityInfrastructure(builder.Configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.Run();
