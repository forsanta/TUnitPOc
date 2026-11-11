namespace TUnitPoc.Domain;

public sealed class OrderService(IStockRepository stock, TimeProvider clock)
{
    public async Task<Order> CheckoutAsync(Cart cart, CancellationToken ct = default)
    {
        if (cart.IsEmpty)
            throw new InvalidOperationException("Impossible de commander un panier vide.");

        foreach (var line in cart.Lines)
        {
            if (await stock.GetAvailableAsync(line.Product.Sku, ct) < line.Quantity)
                throw new OutOfStockException(line.Product.Sku);
        }

        foreach (var line in cart.Lines)
            await stock.ReserveAsync(line.Product.Sku, line.Quantity, ct);

        return new Order(Guid.NewGuid(), cart.Total, clock.GetUtcNow());
    }
}
