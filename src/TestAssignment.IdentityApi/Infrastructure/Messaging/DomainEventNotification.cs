using TestAssignment.ServiceDefaults;

namespace TestAssignment.IdentityApi.Infrastructure.Messaging;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent)
    : IDomainEventNotification<TDomainEvent>
    where TDomainEvent : IDomainEvent;