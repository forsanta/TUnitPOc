namespace TUnitPoc.Domain;

public static class DiscountCalculator
{
    /// <summary>
    /// WELCOME10 : -10 %, BLACKFRIDAY : -30 %, FIXED5 : -5 € (sans passer sous 0).
    /// Les codes sont insensibles à la casse. Un code inconnu est ignoré.
    /// </summary>
    public static decimal Apply(decimal amount, string? code) => code?.ToUpperInvariant() switch
    {
        "WELCOME10" => Math.Round(amount * 0.90m, 2),
        "BLACKFRIDAY" => Math.Round(amount * 0.70m, 2),
        "FIXED5" => Math.Max(0, amount - 5m),
        _ => amount
    };

    /// <summary>Frais de port : gratuits dès 50 €, sinon 4,90 € (8,90 € en express).</summary>
    public static decimal ShippingCost(decimal amount, bool express) => (amount, express) switch
    {
        ( >= 50m, false) => 0m,
        ( >= 50m, true) => 4m,
        (_, false) => 4.90m,
        (_, true) => 8.90m
    };
}
