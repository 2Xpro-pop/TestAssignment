using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TestAssignment.PaymentApi.Domain.Accounts;
using TestAssignment.PaymentApi.Domain.Payments;
using TestAssignment.PaymentApi.Infrastructure.Configurations;
using TestAssignment.PaymentApi.Infrastructure.Messaging;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.PaymentApi.Infrastructure.Persistence;

public sealed class PaymentDbContext(
    DbContextOptions<PaymentDbContext> options,
    DomainEventsDispatcher? domainEventsDispatcher = null)
    : DbContext(options), IUnitOfWork
{
    private readonly DomainEventsDispatcher? _domainEventsDispatcher = domainEventsDispatcher;

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregateRoots = ChangeTracker
            .Entries()
            .Where(entityEntry =>
                entityEntry.Entity is IAggregateRoot aggregateRoot &&
                aggregateRoot.DomainEvents.Count > 0)
            .Select(entityEntry => (IAggregateRoot)entityEntry.Entity)
            .ToArray();

        var domainEvents = aggregateRoots
            .SelectMany(aggregateRoot => aggregateRoot.DomainEvents)
            .ToArray();

        UpdateConcurrencyStamps();

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

    private void UpdateConcurrencyStamps()
    {
        foreach (var entityEntry in ChangeTracker.Entries<Account>())
        {
            if (entityEntry.State is EntityState.Added or EntityState.Modified)
            {
                entityEntry.Property(AccountConfiguration.StampPropertyName).CurrentValue = Guid.NewGuid();
            }
        }
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}