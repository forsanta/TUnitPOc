namespace TUnitPoc.Tests.Orchestration;

/// <summary>
/// [ParallelLimiter] : limite le nombre de tests simultanés (ex. API à quota).
/// </summary>
[ParallelLimiter<MaxTwo>]
public class RateLimitedTests
{
    private static int _running;
    private static int _maxObserved;

    [Test]
    [Repeat(9)]
    public async Task At_most_two_tests_run_concurrently()
    {
        var current = Interlocked.Increment(ref _running);
        InterlockedMax(ref _maxObserved, current);
        await Task.Delay(30);
        Interlocked.Decrement(ref _running);

        await Assert.That(_maxObserved).IsLessThanOrEqualTo(2);
    }

    private static void InterlockedMax(ref int target, int value)
    {
        int snapshot;
        while ((snapshot = target) < value
               && Interlocked.CompareExchange(ref target, value, snapshot) != snapshot)
        {
        }
    }
}
