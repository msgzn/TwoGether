using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwoGether.Domain.Matches;
using TwoGether.Domain.Routes;

namespace TwoGether.Persistence.Configurations;

internal sealed class RouteMatchConfiguration : IEntityTypeConfiguration<RouteMatch>
{
    public void Configure(EntityTypeBuilder<RouteMatch> builder)
    {
        builder.ToTable("route_matches");

        builder.Property(m => m.PickupPoint).HasColumnType("geometry(Point, 4326)");
        builder.Property(m => m.DropoffPoint).HasColumnType("geometry(Point, 4326)");
        builder.Property(m => m.Price).HasPrecision(12, 2);

        builder.HasOne<DriverRoute>().WithMany().HasForeignKey(m => m.DriverRouteId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<PassengerRoute>().WithMany().HasForeignKey(m => m.PassengerRouteId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => new { m.DriverRouteId, m.PassengerRouteId }).IsUnique();
        builder.HasIndex(m => m.DriverId);
        builder.HasIndex(m => m.PassengerId);
    }
}
