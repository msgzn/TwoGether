using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwoGether.Domain.Routes;
using TwoGether.Domain.Users;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Persistence.Configurations;

internal sealed class DriverRouteConfiguration : IEntityTypeConfiguration<DriverRoute>
{
    public void Configure(EntityTypeBuilder<DriverRoute> builder)
    {
        builder.ToTable("driver_routes");

        builder.Property(r => r.Origin).HasColumnType("geometry(Point, 4326)");
        builder.Property(r => r.Destination).HasColumnType("geometry(Point, 4326)");
        builder.Property(r => r.Path).HasColumnType("geometry(LineString, 4326)");

        builder.HasOne<User>().WithMany().HasForeignKey(r => r.DriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Vehicle>().WithMany().HasForeignKey(r => r.VehicleId).OnDelete(DeleteBehavior.Restrict);
    }
}
