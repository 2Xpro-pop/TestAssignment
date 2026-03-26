using TestAssignment.ServiceDefaults;

namespace TestAssignment.PaymentApi.Infrastructure.Messaging;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent)
    : IDomainEventNotification<TDomainEvent>
    where TDomainEvent : IDomainEvent;