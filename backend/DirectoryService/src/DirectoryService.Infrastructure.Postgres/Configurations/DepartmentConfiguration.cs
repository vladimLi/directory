using DirectoryService.Domain;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentConfiguration: IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");
        builder.HasKey(p => p.Id)
            .HasName("pk_departments");

        builder.Property(d => d.Id)
            .HasConversion(d => d.Value , g => DepartmentId.Create(g))
            .HasColumnName("id");
        
        builder.Property(d => d.Name)
            .IsRequired()
            .HasConversion(n => n.Value, name => DepartmentName.Create(name))
            .HasMaxLength(LengthConstants.Length50)
            .HasColumnName("name");
        
        builder.Property(d => d.Slug)
            .IsRequired()
            .HasConversion(s => s.Value, slug => DepartmentSlug.Create(slug))
            .HasMaxLength(LengthConstants.Length100)
            .HasColumnName("slug");
        
        builder.Property(d => d.Path)
            .IsRequired()
            .HasConversion(p => p.Value, path => DepartmentPath.Create(path))
            .HasMaxLength(LengthConstants.Length500)
            .HasColumnName("path");
        
        builder.Property(d => d.ParentId)
            .HasConversion(id => id!.Value, g => DepartmentId.Create(g))
            .HasColumnName("parent_id");
        
        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
    }
}