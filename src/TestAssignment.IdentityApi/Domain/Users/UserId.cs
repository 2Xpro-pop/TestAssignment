using System.Diagnostics;

namespace TestAssignment.IdentityApi.Domain.Users;

/// <summary>
/// Strongly-typed identifier for Curtain aggregate.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct UserId(Guid Value)
{
    /// <summary>
    /// Creates a new unique UserId.
    /// </summary>
    public static UserId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}