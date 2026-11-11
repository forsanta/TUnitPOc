namespace TUnitPoc.Tests.Resilience;

public class ResilienceTests
{
    private static int _flakyAttempts;

    /// <summary>
    /// [Retry] : relance le test en cas d'échec (utile pour les tests d'intégration "flaky").
    /// Ce test échoue volontairement 2 fois puis réussit à la 3e tentative.
    /// </summary>
    [Test]
    [Retry(3)]
    public async Task Flaky_external_call_eventually_succeeds()
    {
        var attempt = Interlocked.Increment(ref _flakyAttempts);
        TestContext.Current!.OutputWriter.WriteLine($"Tentative n°{attempt}");

        await Assert.That(attempt).IsGreaterThanOrEqualTo(3);
    }

    /// <summary>
    /// [Timeout] : le CancellationToken injecté est annulé à l'expiration du délai.
    /// </summary>
    [Test]
    [Timeout(2_000)]
    public async Task Checkout_completes_within_timeout(CancellationToken cancellationToken)
    {
        var stock = new InMemoryStockRepository();
        stock.Set(Products.Mouse.Sku, 10);
        var cart = new Cart();
        cart.Add(Products.Mouse);

        var order = await new OrderService(stock, TimeProvider.System).CheckoutAsync(cart, cancellationToken);

        await Assert.That(order.Amount).IsEqualTo(24.50m);
    }
}
