using TwoGether.Application.Matches;

namespace TwoGether.Application.Tests.Matches;

public class MatchingOptionsTests
{
    private static readonly MatchingOptions Options = new() { PricePerKm = 40, MinimumPrice = 300, PriceRounding = 10 };

    [Fact]
    public void Price_is_per_km_rounded_up()
    {
        Options.CalculatePrice(9_120).Should().Be(370);
    }

    [Fact]
    public void Short_trips_cost_the_minimum()
    {
        Options.CalculatePrice(2_000).Should().Be(300);
    }
}
