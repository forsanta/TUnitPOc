namespace TUnitPoc.Domain;

public interface IStockRepository
{
    Task<int> GetAvailableAsync(string sku, CancellationToken ct = default);
    Task ReserveAsync(string sku, int quantity, CancellationToken ct = default);
}
