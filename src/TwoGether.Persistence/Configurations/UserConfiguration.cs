using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwoGether.Domain.Users;

namespace TwoGether.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(u => u.PhoneNumber)
            .HasConversion<PhoneNumberConverter>()
            .HasMaxLength(PhoneNumberConverter.MaxLength)
            .IsRequired();

        builder.Property(u => u.FirstName).HasMaxLength(User.NameMaxLength);
        builder.Property(u => u.LastName).HasMaxLength(User.NameMaxLength);

        builder.Ignore(u => u.IsProfileCompleted);

        builder.HasIndex(u => u.PhoneNumber).IsUnique().HasFilter("is_deleted = false");
    }
}
