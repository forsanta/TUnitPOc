namespace TUnitPoc.Domain;

public sealed record CartLine(Product Product, int Quantity)
{
    public decimal Total => Product.UnitPrice * Quantity;
}
