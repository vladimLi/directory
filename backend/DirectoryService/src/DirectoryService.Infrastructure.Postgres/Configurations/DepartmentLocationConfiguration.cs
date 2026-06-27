using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using DirectoryService.Domain.Relationships.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public sealed class DepartmentLocationConfiguration: IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_locations");
        builder.HasKey(l => l.Id)
            .HasName("pk_department_locations");
        
        builder.Property(dl => dl.Id)
            .HasConversion(id => id.Value, g => DepartmentLocationId.Create(g))
            .HasColumnName("id");
        builder.Property(dl => dl.DepartmentId)
            .HasConversion(id => id.Value, g => DepartmentId.Create(g))
            .HasColumnName("department_id");
        builder.Property(dl => dl.LocationId)
            .HasConversion(id => id.Value, g => LocationId.Create(g))
            .HasColumnName("location_id");
        builder.Property(dl => dl.IsPrimary)
            .HasColumnName("is_primary")
            .HasDefaultValue(false);
        
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(dl => dl.DepartmentId)
            .IsRequired()
            .HasConstraintName("fk_department_locations_departments")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Location>()
            .WithMany()
            .HasForeignKey(dl => dl.LocationId)
            .IsRequired()
            .HasConstraintName("fk_department_locations_locations")
            .OnDelete(DeleteBehavior.Cascade);
    }
}