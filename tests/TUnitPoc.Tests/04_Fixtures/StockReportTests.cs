namespace TUnitPoc.Tests.Fixtures;

/// <summary>Une autre classe qui réutilise la MÊME instance de fixture.</summary>
[ClassDataSource<StockFixture>(Shared = SharedType.PerTestSession)]
public class StockReportTests(StockFixture fixture)
{
    [Test]
    public async Task Same_fixture_instance_is_shared_across_classes()
    {
        await Assert.That(StockFixture.InitializationCount).IsEqualTo(1);
        await Assert.That(await fixture.Repository.GetAvailableAsync(Products.Cable.Sku)).IsEqualTo(0);
    }
}
