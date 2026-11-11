using TUnit.Core.Interfaces;

namespace TUnitPoc.Tests.Orchestration;

/// <summary>Limite de parallélisme utilisée par [ParallelLimiter&lt;MaxTwo&gt;].</summary>
public record MaxTwo : IParallelLimit
{
    public int Limit => 2;
}
