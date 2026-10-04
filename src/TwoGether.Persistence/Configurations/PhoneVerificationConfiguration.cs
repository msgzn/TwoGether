using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwoGether.Domain.Auth;

namespace TwoGether.Persistence.Configurations;

internal sealed class PhoneVerificationConfiguration : IEntityTypeConfiguration<PhoneVerification>
{
    public void Configure(EntityTypeBuilder<PhoneVerification> builder)
    {
        builder.ToTable("phone_verifications");

        builder.Property(v => v.PhoneNumber)
            .HasConversion<PhoneNumberConverter>()
            .HasMaxLength(PhoneNumberConverter.MaxLength)
            .IsRequired();

        builder.Property(v => v.CodeHash).HasMaxLength(64).IsRequired();

        builder.HasIndex(v => new { v.PhoneNumber, v.SentAt });
    }
}
