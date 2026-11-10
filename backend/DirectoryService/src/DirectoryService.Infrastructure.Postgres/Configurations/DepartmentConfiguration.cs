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
            .HasConversion(d => d.Value , g => DepartmentId.Create(g).Value)
            .HasColumnName("id");
        
        builder.Property(d => d.Name)
            .IsRequired()
            .HasConversion(n => n.Value, name => DepartmentName.Create(name).Value)
            .HasMaxLength(LengthConstants.Length50)
            .HasColumnName("name");
        
        builder.Property(d => d.Slug)
            .IsRequired()
            .HasConversion(s => s.Value, slug => DepartmentSlug.Create(slug).Value)
            .HasMaxLength(LengthConstants.Length100)
            .HasColumnName("slug");
        
        builder.Property(d => d.Path)
            .IsRequired()
            .HasConversion(
                path => path.Value.Replace('/', '.'),
                value => DepartmentPath.Restore(value.Replace('.', '/')))
            .HasColumnType("ltree")
            .HasColumnName("path");
        
        builder.Property(d => d.Depth)
            .IsRequired()
            .HasColumnName("depth");
        
        builder.HasIndex(d => d.Slug)
            .IsUnique()
            .HasDatabaseName("ix_departments_slug_active")
            .HasFilter("is_deleted = false");
        
        builder.HasIndex(d => d.Name)
            .HasDatabaseName("ix_departments_name_trgm")
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
        
        builder.HasIndex(d => d.ParentId)
            .HasDatabaseName("ix_departments_parent_id");
        
        builder.HasIndex(d => d.Path)
            .HasDatabaseName("ix_departments_path_gist")
            .HasMethod("gist");
        
        builder.Property(d => d.ParentId)
            .HasConversion(id => id!.Value, g => DepartmentId.Create(g).Value)
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
        
        builder.Property(d => d.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();
        
        builder.Property(d => d.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone")
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
    }
}