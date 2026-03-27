using Asp.Versioning;
using TestAssignment.PaymentApi.Infrastructure;
using TestAssignment.PaymentApi.V1;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddPaymentInfrastructure(builder.Configuration);

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

var payment = app.NewVersionedApi("payment");

payment.MapPaymentApiV1();

app.Run();