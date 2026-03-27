using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using TestAssignment.PaymentApi.Domain.Accounts;
using TestAssignment.PaymentApi.Domain.Payments;
using TestAssignment.PaymentApi.Infrastructure.Authentication;
using TestAssignment.PaymentApi.Infrastructure.Identity;
using TestAssignment.PaymentApi.Infrastructure.Messaging;
using TestAssignment.PaymentApi.Infrastructure.Persistence;
using TestAssignment.ServiceDefaults;
using TestAssignment.ServiceDefaults.Extensions;

namespace TestAssignment.PaymentApi.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddPaymentInfrastructure(
        this IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        builder.Services.AddAuthentication("IdentityIntrospection")
            .AddScheme<AuthenticationSchemeOptions, IdentityIntrospectionAuthenticationHandler>(
                "IdentityIntrospection",
                _ => { });

        builder.Services.AddAuthorization();

        builder.Services.AddDefaultDbContext<PaymentDbContext, PaymentDbContextSeeder>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("paymentdb"));
        });

        builder.Services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(
                typeof(TestAssignment.PaymentApi.Application.Payments.CreatePayment.CreatePaymentCommand).Assembly);
        });

        builder.Services.AddHttpClient<IIdentityIntrospectionClient, IdentityIntrospectionClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri("http://testassignment-identityapi");
        });

        builder.Services.AddScoped<DomainEventsDispatcher>();

        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

        builder.Services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<PaymentDbContext>());

        builder.Services.AddSingleton(TimeProvider.System);

        return builder;
    }
}