namespace TUnitPoc.Tests;

/// <summary>Horloge figée pour rendre les tests déterministes.</summary>
internal sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}
