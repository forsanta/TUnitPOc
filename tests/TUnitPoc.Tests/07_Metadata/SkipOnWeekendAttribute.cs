namespace TUnitPoc.Tests.Metadata;

/// <summary>Skip conditionnel personnalisé : il suffit d'hériter de SkipAttribute.</summary>
public class SkipOnWeekendAttribute() : SkipAttribute("Pas de tests de prod le week-end")
{
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
        => Task.FromResult(DateTime.Today.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
}
