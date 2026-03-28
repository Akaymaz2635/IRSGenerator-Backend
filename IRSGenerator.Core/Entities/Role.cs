using System.Collections.ObjectModel;

namespace IRSGenerator.Core.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = default!;

    public ICollection<RolePermission> RolePermissions { get; set; }
        = new Collection<RolePermission>();
    public ICollection<UserRole> UserRoles { get; set; }
        = new Collection<UserRole>();
}
