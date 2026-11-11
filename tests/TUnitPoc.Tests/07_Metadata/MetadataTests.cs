namespace TUnitPoc.Tests.Metadata;

/// <summary>
/// Catégories, propriétés, noms d'affichage et skip.
/// Filtrage en CLI : dotnet run -- --treenode-filter "/*/*/*/*[Category=Pricing]"
/// </summary>
[Category("Pricing")]
public class MetadataTests
{
    [Test]
    [DisplayName("Le code WELCOME10 retire 10 % de $amount €")]
    [Arguments(200)]
    [Arguments(42)]
    public async Task Welcome_discount(decimal amount)
    {
        await Assert.That(DiscountCalculator.Apply(amount, "WELCOME10"))
            .IsEqualTo(Math.Round(amount * 0.9m, 2));
    }

    [Test]
    [Property("Owner", "team-checkout")]
    [Property("Jira", "SHOP-1234")]
    public async Task Properties_are_readable_from_context()
    {
        var properties = TestContext.Current!.Metadata.TestDetails.CustomProperties;

        await Assert.That(properties["Owner"]).Contains("team-checkout");
    }

    [Test]
    [Skip("Le paiement en 3 fois n'est pas encore implémenté")]
    public void Installment_payment()
    {
    }

    [Test]
    [SkipOnWeekend]
    public async Task Custom_skip_attribute()
    {
        await Assert.That(DateTime.Today.DayOfWeek).IsNotEqualTo(DayOfWeek.Sunday);
    }
}
