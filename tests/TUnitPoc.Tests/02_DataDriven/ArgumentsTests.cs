namespace TUnitPoc.Tests.DataDriven;

/// <summary>[Arguments] : l'équivalent de [InlineData] (xUnit) / [TestCase] (NUnit).</summary>
public class ArgumentsTests
{
    [Test]
    [Arguments(100, "WELCOME10", 90)]
    [Arguments(100, "welcome10", 90)]      // insensible à la casse
    [Arguments(100, "BLACKFRIDAY", 70)]
    [Arguments(3, "FIXED5", 0)]            // jamais sous zéro
    [Arguments(100, "INCONNU", 100)]
    [Arguments(100, null, 100)]
    public async Task Discount_codes_are_applied(decimal amount, string? code, decimal expected)
    {
        var result = DiscountCalculator.Apply(amount, code);

        await Assert.That(result).IsEqualTo(expected);
    }
}
