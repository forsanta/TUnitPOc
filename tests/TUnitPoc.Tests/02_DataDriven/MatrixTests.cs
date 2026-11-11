namespace TUnitPoc.Tests.DataDriven;

/// <summary>
/// [Matrix] : produit cartésien des valeurs de chaque paramètre.
/// Ici 4 montants x 2 modes = 8 tests générés.
/// </summary>
public class MatrixTests
{
    [Test]
    [MatrixDataSource]
    public async Task Shipping_is_never_negative_nor_above_9_euros(
        [Matrix(0, 10, 49.99, 50)] decimal amount,
        [Matrix(true, false)] bool express)
    {
        var shipping = DiscountCalculator.ShippingCost(amount, express);

        await Assert.That(shipping).IsBetween(0m, 9m);
    }

    [Test]
    [MatrixDataSource]
    public async Task Express_always_costs_more(
        [Matrix(0, 10, 49.99, 50, 120)] decimal amount)
    {
        var standard = DiscountCalculator.ShippingCost(amount, express: false);
        var express = DiscountCalculator.ShippingCost(amount, express: true);

        await Assert.That(express).IsGreaterThan(standard);
    }
}
