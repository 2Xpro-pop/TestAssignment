using MediatR;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.IdentityApi.Infrastructure.Messaging;

public sealed class DomainEventsDispatcher(IPublisher publisher)
{
    private readonly IPublisher _publisher = publisher;

    public async Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());


            if (Activator.CreateInstance(notificationType, domainEvent) is not INotification notification)
            {
                throw new InvalidOperationException(
                    $"Could not create domain event notification for '{domainEvent.GetType().FullName}'.");
            }

            await _publisher.Publish(notification, cancellationToken);
        }
    }
}