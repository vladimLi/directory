using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentDepthAndLtree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:ltree", ",,");

            migrationBuilder.Sql("""
                ALTER TABLE departments
                ALTER COLUMN path TYPE ltree
                USING replace(path, '/', '.')::ltree;
                """);

            migrationBuilder.AddColumn<int>(
                name: "depth",
                table: "departments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE departments
                SET depth = nlevel(path) - 1;
                """);

            migrationBuilder.CreateIndex(
                name: "ix_departments_parent_id",
                table: "departments",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_departments_path_gist",
                table: "departments",
                column: "path")
                .Annotation("Npgsql:IndexMethod", "gist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_departments_parent_id",
                table: "departments");

            migrationBuilder.DropIndex(
                name: "ix_departments_path_gist",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "depth",
                table: "departments");

            migrationBuilder.Sql("""
                ALTER TABLE departments
                ALTER COLUMN path TYPE varchar(500)
                USING replace(path::text, '.', '/');
                """);
        }
    }
}
