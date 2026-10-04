namespace TwoGether.Application.Abstractions;

/// <summary>Names of the global EF query filters, for <c>IgnoreQueryFilters([...])</c>.</summary>
public static class QueryFilters
{
    /// <summary>Hides rows with IsDeleted = true. Applied to every entity by default.</summary>
    public const string SoftDelete = nameof(SoftDelete);
}
