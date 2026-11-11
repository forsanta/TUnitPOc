namespace TUnitPoc.Domain;

public sealed record Order(Guid Id, decimal Amount, DateTimeOffset CreatedAt);
