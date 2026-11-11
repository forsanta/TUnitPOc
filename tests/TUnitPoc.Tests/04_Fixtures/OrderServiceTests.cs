namespace TUnitPoc.Tests.Fixtures;

/// <summary>
/// [ClassDataSource] injecte la fixture par constructeur.
/// Shared = PerTestSession : une seule instance pour toute la session,
/// partagée avec les autres classes qui la demandent.
/// </summary>
[ClassDataSource<StockFixture>(Shared = SharedType.PerTestSession)]
public class OrderServiceTests(StockFixture fixture)
{
    private static readonly DateTimeOffset Now = new(2026, 11, 27, 9, 0, 0, TimeSpan.Zero);

    private OrderService CreateService() => new(fixture.Repository, new FixedTimeProvider(Now));

    [Test]
    public async Task Checkout_creates_an_order()
    {
        var cart = new Cart();
        cart.Add(Products.Keyboard);
        cart.ApplyDiscount("BLACKFRIDAY");

        var order = await CreateService().CheckoutAsync(cart);

        using (Assert.Multiple())
        {
            await Assert.That(order.Id).IsNotEqualTo(Guid.Empty);
            await Assert.That(order.Amount).IsEqualTo(62.93m);
            await Assert.That(order.CreatedAt).IsEqualTo(Now);
        }
    }

    [Test]
    public async Task Checkout_fails_when_out_of_stock()
    {
        var cart = new Cart();
        cart.Add(Products.Cable);

        var exception = await Assert.That(() => CreateService().CheckoutAsync(cart))
            .Throws<OutOfStockException>();

        await Assert.That(exception!.Sku).IsEqualTo(Products.Cable.Sku);
    }

    [Test]
    public async Task Checkout_of_empty_cart_is_forbidden()
    {
        await Assert.That(() => CreateService().CheckoutAsync(new Cart()))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task Fixture_is_initialized_once_per_session()
    {
        await Assert.That(StockFixture.InitializationCount).IsEqualTo(1);
    }
}
