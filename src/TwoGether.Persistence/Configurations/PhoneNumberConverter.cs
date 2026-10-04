using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TwoGether.Domain.Users;

namespace TwoGether.Persistence.Configurations;

internal sealed class PhoneNumberConverter() : ValueConverter<PhoneNumber, string>(
    p => p.Value,
    v => PhoneNumber.FromStorage(v))
{
    public const int MaxLength = 16;
}
