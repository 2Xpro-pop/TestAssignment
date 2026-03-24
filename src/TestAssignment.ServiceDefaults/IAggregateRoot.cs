using System;
using System.Collections.Generic;
using System.Text;

namespace TestAssignment.ServiceDefaults;

public interface IAggregateRoot
{
    public IReadOnlyCollection<IDomainEvent> DomainEvents
    {
        get;
    }

    public void ClearDomainEvents();
}