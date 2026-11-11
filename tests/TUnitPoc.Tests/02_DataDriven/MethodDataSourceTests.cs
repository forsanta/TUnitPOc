namespace TUnitPoc.Tests.DataDriven;

/// <summary>
/// [MethodDataSource] : données construites par du code (objets complexes, etc.).
/// Chaque élément est un Func&lt;T&gt; pour que chaque test reçoive une instance neuve.
/// </summary>
public class MethodDataSourceTests
{
    public static IEnumerable<Func<(Cart Cart, decimal ExpectedTotal)>> Carts()
    {
        yield return () => (new Cart(), 0m);
        yield return () =>
        {
            var cart = new Cart();
            cart.Add(Products.Keyboard);
            return (cart, 89.90m);
        };
        yield return () =>
        {
            var cart = new Cart();
            cart.Add(Products.Keyboard);
            cart.Add(Products.Mouse, 2);
            cart.ApplyDiscount("WELCOME10");
            return (cart, 125.01m); // (89,90 + 2 x 24,50) - 10 %
        };
    }

    [Test]
    [MethodDataSource(nameof(Carts))]   // le tuple est "déplié" dans les paramètres
    public async Task Cart_total_is_computed(Cart cart, decimal expectedTotal)
    {
        await Assert.That(cart.Total).IsEqualTo(expectedTotal);
    }
}
