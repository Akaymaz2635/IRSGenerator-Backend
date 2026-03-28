namespace IRSGenerator.Core.Entities
{
    /// <summary>
    /// Mevcut User entity'sine eklenecek yeni alanlar:
    /// PasswordHash, Role, Active
    /// Bu dosyayı doğrudan kopyalamak yerine eksik alanları mevcut User.cs'e ekleyin.
    /// </summary>
    public class User : BaseEntity
    {
        public string EmployeeId { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string WindowsAccount { get; set; } = default!;
        public string DisplayName { get; set; } = default!;

        // ── QualiSight için yeni alanlar ──────────────────────────────────
        public string? PasswordHash { get; set; }
        public string Role { get; set; } = "inspector";   // inspector | engineer | admin
        public bool Active { get; set; } = true;

        public ICollection<UserRole> UserRoles { get; set; }
            = new System.Collections.ObjectModel.Collection<UserRole>();
    }
}
