using System;
using System.Collections.Generic;
using System.Text;

namespace TestAssignment.ServiceDefaults;

/// <summary>
/// Marker interface for domain events.
/// </summary>
public interface IDomainEvent
{
    public DateTimeOffset OccurredAtUtc { get; }
}