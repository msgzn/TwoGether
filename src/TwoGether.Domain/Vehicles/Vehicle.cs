using System.Text.RegularExpressions;
using TwoGether.Domain.Common;

namespace TwoGether.Domain.Vehicles;

public sealed partial class Vehicle : AggregateRoot
{
    public const int TextMaxLength = 50;
    public const int MinPassengerSeats = 1;
    public const int MaxPassengerSeats = 6;

    private Vehicle() { }

    public Guid OwnerId { get; private init; }
    public string Make { get; private set; } = null!;
    public string Model { get; private set; } = null!;
    public string Color { get; private set; } = null!;
    public string PlateNumber { get; private set; } = null!;
    public int PassengerSeats { get; private set; }

    public static Vehicle Register(Guid ownerId, string make, string model, string color, string plateNumber, int passengerSeats)
    {
        var vehicle = new Vehicle { OwnerId = ownerId };
        vehicle.Update(make, model, color, plateNumber, passengerSeats);
        return vehicle;
    }

    public void Update(string make, string model, string color, string plateNumber, int passengerSeats)
    {
        if (passengerSeats is < MinPassengerSeats or > MaxPassengerSeats)
            throw new DomainException("vehicle.invalid_seats", $"Passenger seats must be between {MinPassengerSeats} and {MaxPassengerSeats}.");

        Make = Required(make, "vehicle.make_required");
        Model = Required(model, "vehicle.model_required");
        Color = Required(color, "vehicle.color_required");
        PlateNumber = NormalizePlate(plateNumber);
        PassengerSeats = passengerSeats;
    }

    public bool IsOwnedBy(Guid userId) => OwnerId == userId;

    public static string NormalizePlate(string plateNumber)
    {
        var plate = PlateSeparators().Replace(plateNumber ?? "", "").ToUpperInvariant();

        if (!PlateFormat().IsMatch(plate))
            throw new DomainException("vehicle.invalid_plate", "Plate number must contain 5 to 9 letters and digits, e.g. 123ABC02.");

        return plate;
    }

    private static string Required(string value, string errorCode)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(errorCode, "Value is required.");

        value = value.Trim();
        if (value.Length > TextMaxLength)
            throw new DomainException("vehicle.text_too_long", $"Value must be at most {TextMaxLength} characters.");

        return value;
    }

    [GeneratedRegex(@"[\s\-]")]
    private static partial Regex PlateSeparators();

    [GeneratedRegex("^[0-9A-Z]{5,9}$")]
    private static partial Regex PlateFormat();
}
