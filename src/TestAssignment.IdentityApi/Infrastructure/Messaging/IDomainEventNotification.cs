using MediatR;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.IdentityApi.Infrastructure.Messaging;

public interface IDomainEventNotification<out TDomainEvent> : INotification
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; }
}