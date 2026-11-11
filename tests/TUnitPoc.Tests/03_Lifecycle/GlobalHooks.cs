namespace TUnitPoc.Tests.Lifecycle;

/// <summary>
/// Hooks globaux : s'exécutent pour toute la session / l'assembly,
/// sans héritage ni classe de base.
/// </summary>
public static class GlobalHooks
{
    [Before(TestSession)]
    public static void SessionStart()
        => Console.WriteLine(">> Démarrage de la session de tests TUnit");

    [After(TestSession)]
    public static void SessionEnd(TestSessionContext context)
        => Console.WriteLine($">> Fin de session : {context.AllTests.Count()} tests exécutés");

    // [BeforeEvery(Test)] s'exécuterait avant CHAQUE test de l'assembly
}
