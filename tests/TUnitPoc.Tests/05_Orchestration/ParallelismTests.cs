namespace TUnitPoc.Tests.Orchestration;

/// <summary>
/// Par défaut, TUnit exécute TOUS les tests en parallèle.
/// [NotInParallel("clé")] : les tests partageant la même clé sont sérialisés.
/// </summary>
[NotInParallel("shared-counter")]
public class ParallelismTests
{
    // Ressource globale volontairement non thread-safe
    private static int _sharedCounter;

    [Test]
    [Repeat(5)]   // 6 exécutions (1 + 5 répétitions), jamais en même temps
    public async Task Non_thread_safe_resource_is_protected()
    {
        var before = _sharedCounter;
        await Task.Delay(10);
        _sharedCounter = before + 1;

        await Assert.That(_sharedCounter).IsEqualTo(before + 1);
    }
}
