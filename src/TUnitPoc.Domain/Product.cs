namespace TUnitPoc.Domain;

public sealed record Product(string Sku, string Name, decimal UnitPrice)
{
    public decimal UnitPrice { get; } = UnitPrice >= 0
        ? UnitPrice
        : throw new ArgumentOutOfRangeException(nameof(UnitPrice), "Le prix ne peut pas être négatif.");
}
