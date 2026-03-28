namespace IRSGenerator.Core.Entities;

public class RolePermission : BaseEntity
{
    public long RoleId { get; set; }
    public Role? Role { get; set; }

    public long PermissionId { get; set; }
    public Permission? Permission { get; set; }
}
