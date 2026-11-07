namespace DirectoryService.Core.Database;

public static class SoftDeleteSql
{
    public const string DepartmentActive = "d.is_deleted = false";
    public const string LocationActive = "l.is_deleted = false";
    public const string PositionActive = "p.is_deleted = false";
}