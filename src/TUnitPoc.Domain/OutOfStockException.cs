namespace TUnitPoc.Domain;

public sealed class OutOfStockException(string sku) : Exception($"Stock insuffisant pour '{sku}'.")
{
    public string Sku { get; } = sku;
}
