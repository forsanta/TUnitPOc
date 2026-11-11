namespace TUnitPoc.Domain;

public sealed class Cart
{
    private readonly Dictionary<string, CartLine> _lines = new();

    public IReadOnlyCollection<CartLine> Lines => _lines.Values;

    public bool IsEmpty => _lines.Count == 0;

    public string? DiscountCode { get; private set; }

    public void Add(Product product, int quantity = 1)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "La quantité doit être strictement positive.");

        _lines[product.Sku] = _lines.TryGetValue(product.Sku, out var line)
            ? line with { Quantity = line.Quantity + quantity }
            : new CartLine(product, quantity);
    }

    public void Remove(string sku)
    {
        if (!_lines.Remove(sku))
            throw new InvalidOperationException($"Le produit '{sku}' n'est pas dans le panier.");
    }

    public void ApplyDiscount(string code) => DiscountCode = code;

    public decimal SubTotal => _lines.Values.Sum(l => l.Total);

    public decimal Total => DiscountCalculator.Apply(SubTotal, DiscountCode);
}
