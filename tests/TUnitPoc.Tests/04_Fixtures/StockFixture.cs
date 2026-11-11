using TUnit.Core.Interfaces;

namespace TUnitPoc.Tests.Fixtures;

/// <summary>
/// Simule une ressource coûteuse (base de données, conteneur Docker...).
/// IAsyncInitializer : TUnit appelle InitializeAsync avant la première utilisation.
/// IAsyncDisposable : appelé quand plus aucun test n'en a besoin.
/// </summary>
public sealed class StockFixture : IAsyncInitializer, IAsyncDisposable
{
    public static int InitializationCount;

    public InMemoryStockRepository Repository { get; } = new();

    public async Task InitializeAsync()
    {
        Interlocked.Increment(ref InitializationCount);
        await Task.Delay(200); // "démarrage" de la ressource
        Repository.Set(Products.Keyboard.Sku, 1_000);
        Repository.Set(Products.Mouse.Sku, 1_000);
        Repository.Set(Products.Cable.Sku, 0);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
