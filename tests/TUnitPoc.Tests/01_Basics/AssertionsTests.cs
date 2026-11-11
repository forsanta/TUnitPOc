namespace TUnitPoc.Tests.Basics;

/// <summary>
/// Les bases : [Test], assertions asynchrones et fluides.
/// Pas d'attribut sur la classe, pas de [TestClass] / [TestFixture].
/// </summary>
public class AssertionsTests
{
    [Test]
    public async Task New_cart_is_empty()
    {
        var cart = new Cart();

        // Toutes les assertions TUnit sont awaitées
        await Assert.That(cart.IsEmpty).IsTrue();
        await Assert.That(cart.Total).IsEqualTo(0m);
    }

    [Test]
    public async Task Adding_same_product_twice_merges_lines()
    {
        var cart = new Cart();

        cart.Add(Products.Keyboard);
        cart.Add(Products.Keyboard, 2);

        await Assert.That(cart.Lines).HasSingleItem();
        await Assert.That(cart.Lines.Single().Quantity).IsEqualTo(3);
    }

    [Test]
    public async Task Assertions_can_be_chained_with_And()
    {
        var cart = new Cart();
        cart.Add(Products.Mouse, 2);

        await Assert.That(cart.Total)
            .IsGreaterThan(40m)
            .And.IsLessThan(50m)
            .And.IsEqualTo(49m);
    }

    [Test]
    public async Task Assert_Multiple_reports_every_failure_at_once()
    {
        var cart = new Cart();
        cart.Add(Products.Keyboard);
        cart.Add(Products.Cable, 3);

        // Toutes les assertions du bloc sont évaluées, les échecs sont agrégés
        using (Assert.Multiple())
        {
            await Assert.That(cart.Lines).Count().IsEqualTo(2);
            await Assert.That(cart.SubTotal).IsEqualTo(119.87m);
            await Assert.That(cart.DiscountCode).IsNull();
        }
    }

    [Test]
    public async Task Removing_unknown_product_throws()
    {
        var cart = new Cart();

        await Assert.That(() => cart.Remove("NOPE"))
            .Throws<InvalidOperationException>()
            .WithMessageContaining("NOPE");
    }

    [Test]
    public async Task Negative_quantity_is_rejected()
    {
        var cart = new Cart();

        var exception = await Assert.That(() => cart.Add(Products.Mouse, -1))
            .ThrowsExactly<ArgumentOutOfRangeException>();

        // L'assertion renvoie l'exception : on peut continuer à l'inspecter
        await Assert.That(exception!.ParamName).IsEqualTo("quantity");
    }
}
