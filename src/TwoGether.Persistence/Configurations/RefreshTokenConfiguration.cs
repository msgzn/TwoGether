using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwoGether.Domain.Auth;
using TwoGether.Domain.Users;

namespace TwoGether.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();

        builder.Ignore(t => t.IsRevoked);

        builder.HasOne<User>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.TokenHash).IsUnique();
    }
}
