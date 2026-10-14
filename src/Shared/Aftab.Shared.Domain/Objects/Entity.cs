using Aftab.Shared.Domain.PrefixedGuidTools;

namespace Aftab.Shared.Domain.Objects;

public abstract class Entity<T> : Entity where T : PrefixedGuidV3
{
    public T Id { get; protected init; } = PrefixedGuidV3.New<T>();
}

public abstract class Entity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; }
    public DateTime DeletedAt { get; protected set; }
}
