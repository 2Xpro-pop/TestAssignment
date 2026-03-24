using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TestAssignment.IdentityApi.Domain.Users;
using TestAssignment.IdentityApi.Infrastructure.Configurations;
using TestAssignment.IdentityApi.Infrastructure.Messaging;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.IdentityApi.Infrastructure.Persistence;

public sealed class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options,
    DomainEventsDispatcher? domainEventsDispatcher = null)
    : DbContext(options), IUnitOfWork
{
    private readonly DomainEventsDispatcher? _domainEventsDispatcher = domainEventsDispatcher;

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregateRoots = ChangeTracker
            .Entries()
            .Where(entityEntry => entityEntry.Entity is IAggregateRoot aggregateRoot
                && aggregateRoot.DomainEvents.Count > 0)
            .Select(entityEntry => (IAggregateRoot)entityEntry.Entity)
            .ToArray();

        var domainEvents = aggregateRoots
            .SelectMany(aggregateRoot => aggregateRoot.DomainEvents)
            .ToArray();

        var savedChangesCount = await base.SaveChangesAsync(cancellationToken);

        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.ClearDomainEvents();
        }

        if (_domainEventsDispatcher is not null && domainEvents.Length != 0)
        {
            await _domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);
        }

        return savedChangesCount;
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}