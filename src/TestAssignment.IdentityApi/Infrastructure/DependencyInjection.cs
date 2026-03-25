using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestAssignment.IdentityApi.Application.Users;
using TestAssignment.IdentityApi.Application.Users.Commands.Login;
using TestAssignment.IdentityApi.Domain.Sesssions;
using TestAssignment.IdentityApi.Domain.Users;
using TestAssignment.IdentityApi.Infrastructure.Messaging;
using TestAssignment.IdentityApi.Infrastructure.Persistence;
using TestAssignment.IdentityApi.Infrastructure.Users;
using TestAssignment.IdentityApi.Infrastructure.UserSessions;
using TestAssignment.ServiceDefaults;
using TestAssignment.ServiceDefaults.Extensions;

namespace TestAssignment.IdentityApi.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddIdentityInfrastructure(
        this IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("identitydb")
            ?? throw new InvalidOperationException("Connection string 'identitydb' was not found.");

        builder.Services.AddScoped<DomainEventsDispatcher>();

        builder.Services.AddDefaultDbContext<IdentityDbContext, IdentityDbContextSeeder>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        builder.Services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<IdentityDbContext>());

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();

        builder.Services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();

        builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();
        builder.Services.AddSingleton<ITokenHasher, TokenHasher>();

        builder.Services.Configure<PasswordHasherOptions>(options =>
        {
            options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
            options.IterationCount = 120_000;
        });

        builder.Services.AddSingleton(TimeProvider.System);

        builder.Services.AddMediatR(mediatRServiceConfiguration =>
        {
            mediatRServiceConfiguration.RegisterServicesFromAssemblies(
                typeof(LoginCommandHandler).Assembly,
                typeof(DependencyInjection).Assembly);
        });

        return builder;
    }

    
}