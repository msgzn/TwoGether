using TwoGether.Domain.Routes;

namespace TwoGether.Application.Routes;

public static class RideDaysMapper
{
    private static readonly (DayOfWeek Day, RideDays Flag)[] Map =
    [
        (DayOfWeek.Monday, RideDays.Monday),
        (DayOfWeek.Tuesday, RideDays.Tuesday),
        (DayOfWeek.Wednesday, RideDays.Wednesday),
        (DayOfWeek.Thursday, RideDays.Thursday),
        (DayOfWeek.Friday, RideDays.Friday),
        (DayOfWeek.Saturday, RideDays.Saturday),
        (DayOfWeek.Sunday, RideDays.Sunday),
    ];

    public static RideDays ToRideDays(IEnumerable<DayOfWeek> days) =>
        days.Aggregate(RideDays.None, (result, day) => result | Map.Single(m => m.Day == day).Flag);

    public static IReadOnlyList<DayOfWeek> ToDays(RideDays days) =>
        Map.Where(m => days.HasFlag(m.Flag)).Select(m => m.Day).ToList();
}
