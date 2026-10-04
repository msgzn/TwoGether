using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwoGether.Domain.Routes;
using TwoGether.Domain.Users;

namespace TwoGether.Persistence.Configurations;

internal sealed class PassengerRouteConfiguration : IEntityTypeConfiguration<PassengerRoute>
{
    public void Configure(EntityTypeBuilder<PassengerRoute> builder)
    {
        builder.ToTable("passenger_routes");

        builder.Property(r => r.Origin).HasColumnType("geometry(Point, 4326)");
        builder.Property(r => r.Destination).HasColumnType("geometry(Point, 4326)");

        builder.HasOne<User>().WithMany().HasForeignKey(r => r.PassengerId).OnDelete(DeleteBehavior.Restrict);
    }
}
