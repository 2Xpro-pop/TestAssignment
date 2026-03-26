using MediatR;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.PaymentApi.Infrastructure.Messaging;

public interface IDomainEventNotification<out TDomainEvent> : INotification
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; }
}