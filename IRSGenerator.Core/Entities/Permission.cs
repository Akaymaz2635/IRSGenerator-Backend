using System.Collections.ObjectModel;

namespace IRSGenerator.Core.Entities;

public class Permission : BaseEntity
{
    public string Code { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; }
        = new Collection<RolePermission>();
}
