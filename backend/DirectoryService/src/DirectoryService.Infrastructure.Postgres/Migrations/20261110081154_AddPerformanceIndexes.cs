using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        private static readonly string[] GinTrgmOps = ["gin_trgm_ops"];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_department_positions_position_id",
                table: "department_positions",
                newName: "ix_department_positions_position_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_positions_department_id",
                table: "department_positions",
                newName: "ix_department_positions_department_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_locations_location_id",
                table: "department_locations",
                newName: "ix_department_locations_location_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_locations_department_id",
                table: "department_locations",
                newName: "ix_department_locations_department_id");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:ltree", ",,")
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:ltree", ",,");

            migrationBuilder.CreateIndex(
                name: "ix_departments_name_trgm",
                table: "departments",
                column: "name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", GinTrgmOps);

            migrationBuilder.CreateIndex(
                name: "ix_departments_slug_active",
                table: "departments",
                column: "slug",
                unique: true,
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_departments_name_trgm",
                table: "departments");

            migrationBuilder.DropIndex(
                name: "ix_departments_slug_active",
                table: "departments");

            migrationBuilder.RenameIndex(
                name: "ix_department_positions_position_id",
                table: "department_positions",
                newName: "IX_department_positions_position_id");

            migrationBuilder.RenameIndex(
                name: "ix_department_positions_department_id",
                table: "department_positions",
                newName: "IX_department_positions_department_id");

            migrationBuilder.RenameIndex(
                name: "ix_department_locations_location_id",
                table: "department_locations",
                newName: "IX_department_locations_location_id");

            migrationBuilder.RenameIndex(
                name: "ix_department_locations_department_id",
                table: "department_locations",
                newName: "IX_department_locations_department_id");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:ltree", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:ltree", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,");
        }
    }
}
