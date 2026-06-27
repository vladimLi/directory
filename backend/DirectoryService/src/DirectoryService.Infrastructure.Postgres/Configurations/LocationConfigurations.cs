using DirectoryService.Domain;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class LocationConfigurations: IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        builder.HasKey(l => l.Id)
            .HasName("pk_locations");
        builder.Property(l => l.Id)
            .HasConversion(id => id.Value, g => LocationId.Create(g))
            .HasColumnName("id");
        builder.Property(l => l.Name)
            .IsRequired()
            .HasConversion(n => n.Value, n => LocationName.Create(n))
            .HasMaxLength(LengthConstants.Length50)
            .HasColumnName("location_name");
        builder.Property(l => l.Address)
            .HasMaxLength(LengthConstants.Length500)
            .IsRequired()
            .HasConversion(a => a.Value, a => LocationAddress.Create(a))
            .HasColumnName("location_address");
        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
    }
}