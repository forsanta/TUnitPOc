namespace TUnitPoc.Tests.Orchestration;

/// <summary>
/// [DependsOn] : un test attend la fin (et le succès) d'un autre.
/// Si le test parent échoue, le dépendant est ignoré au lieu d'échouer en cascade.
/// </summary>
public class DependsOnTests
{
    private static readonly InMemoryStockRepository Stock = new();

    [Test]
    public async Task Step1_stock_is_provisioned()
    {
        Stock.Set("SCENARIO", 2);
        await Assert.That(await Stock.GetAvailableAsync("SCENARIO")).IsEqualTo(2);
    }

    [Test]
    [DependsOn(nameof(Step1_stock_is_provisioned))]
    public async Task Step2_reservation_decrements_stock()
    {
        await Stock.ReserveAsync("SCENARIO", 2);
        await Assert.That(await Stock.GetAvailableAsync("SCENARIO")).IsEqualTo(0);
    }

    [Test]
    [DependsOn(nameof(Step2_reservation_decrements_stock))]
    public async Task Step3_next_checkout_is_out_of_stock()
    {
        var cart = new Cart();
        cart.Add(new Product("SCENARIO", "Produit scénario", 10m));
        var service = new OrderService(Stock, TimeProvider.System);

        await Assert.That(() => service.CheckoutAsync(cart)).Throws<OutOfStockException>();
    }
}
