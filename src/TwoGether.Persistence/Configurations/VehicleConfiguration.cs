using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwoGether.Domain.Users;
using TwoGether.Domain.Vehicles;

namespace TwoGether.Persistence.Configurations;

internal sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");

        builder.Property(v => v.Make).HasMaxLength(Vehicle.TextMaxLength).IsRequired();
        builder.Property(v => v.Model).HasMaxLength(Vehicle.TextMaxLength).IsRequired();
        builder.Property(v => v.Color).HasMaxLength(Vehicle.TextMaxLength).IsRequired();
        builder.Property(v => v.PlateNumber).HasMaxLength(9).IsRequired();

        builder.HasOne<User>().WithMany().HasForeignKey(v => v.OwnerId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.PlateNumber).IsUnique().HasFilter("is_deleted = false");
    }
}
