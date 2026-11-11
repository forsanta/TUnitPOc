namespace TUnitPoc.Tests.Lifecycle;

/// <summary>
/// Hooks [Before]/[After] à plusieurs niveaux : Test, Class, Assembly, TestSession.
/// Une nouvelle instance de classe est créée pour chaque test (comme xUnit).
/// </summary>
public class HooksTests
{
    private static int _classSetupCalls;
    private Cart _cart = null!;

    [Before(Class)]
    public static Task BeforeClass(ClassHookContext context)
    {
        _classSetupCalls++;
        return Task.CompletedTask;
    }

    [Before(Test)]
    public void CreateCart()
    {
        _cart = new Cart();
        _cart.Add(Products.Cable);
    }

    [After(Test)]
    public async Task Log(TestContext context)
    {
        // Le contexte expose le résultat du test en cours
        await context.OutputWriter.WriteLineAsync(
            $"[{context.Metadata.TestName}] -> {context.Execution.Result?.State}");
    }

    [Test]
    public async Task Each_test_gets_a_fresh_cart_1()
    {
        _cart.Add(Products.Mouse);
        await Assert.That(_cart.Lines).Count().IsEqualTo(2);
    }

    [Test]
    public async Task Each_test_gets_a_fresh_cart_2()
    {
        // Le produit ajouté par l'autre test n'est pas là
        await Assert.That(_cart.Lines).Count().IsEqualTo(1);
    }

    [Test]
    public async Task Class_hook_runs_only_once()
    {
        await Assert.That(_classSetupCalls).IsEqualTo(1);
    }
}
