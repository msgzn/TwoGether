namespace TwoGether.Application.Matches;

public sealed class MatchingOptions
{
    public const string SectionName = "Matching";

    public double MaxWalkMeters { get; set; } = 500;
    public decimal PricePerKm { get; set; }
    public decimal MinimumPrice { get; set; }
    public decimal PriceRounding { get; set; } = 10;

    public decimal CalculatePrice(double sharedDistanceMeters)
    {
        var raw = (decimal)(sharedDistanceMeters / 1000) * PricePerKm;
        var rounded = PriceRounding > 0 ? Math.Ceiling(raw / PriceRounding) * PriceRounding : raw;
        return Math.Max(MinimumPrice, rounded);
    }
}
