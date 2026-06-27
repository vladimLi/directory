using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Domain.Relationships;
using DirectoryService.Domain.Relationships.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentPositionConfiguration:IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_positions");
        builder.HasKey(dp => dp.Id)
            .HasName("pk_department_position_id");

        builder.Property(dp => dp.Id)
            .HasConversion(id => id.Value, g => DepartmentPositionId.Create(g))
            .HasColumnName("id");
        builder.Property(dp => dp.DepartmentId)
            .HasConversion(id => id.Value, g => DepartmentId.Create(g))
            .HasColumnName("department_id");
        builder.Property(dp => dp.PositionId)
            .HasConversion(id => id.Value, g => PositionId.Create(g))
            .HasColumnName("position_id");
        
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(dp => dp.DepartmentId)
            .IsRequired()
            .HasConstraintName("fk_department_position_departments")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Position>()
            .WithMany()
            .HasForeignKey(dp => dp.PositionId)
            .IsRequired()
            .HasConstraintName("fk_department_position_positions")
            .OnDelete(DeleteBehavior.Cascade);
    }
}