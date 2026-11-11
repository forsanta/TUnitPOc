namespace TUnitPoc.Domain;

/// <summary>Implémentation en mémoire, thread-safe, utilisée par les tests.</summary>
public sealed class InMemoryStockRepository : IStockRepository
{
    private readonly Dictionary<string, int> _stock = new();
    private readonly Lock _lock = new();

    public void Set(string sku, int quantity)
    {
        lock (_lock) _stock[sku] = quantity;
    }

    public Task<int> GetAvailableAsync(string sku, CancellationToken ct = default)
    {
        lock (_lock) return Task.FromResult(_stock.GetValueOrDefault(sku));
    }

    public Task ReserveAsync(string sku, int quantity, CancellationToken ct = default)
    {
        lock (_lock) _stock[sku] = _stock.GetValueOrDefault(sku) - quantity;
        return Task.CompletedTask;
    }
}
