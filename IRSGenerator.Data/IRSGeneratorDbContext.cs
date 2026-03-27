using IRSGenerator.Core;
using IRSGenerator.Core.Entities;
using IRSGenerator.Data.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IRSGenerator.Data
{
    public class IRSGeneratorDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<IRSProject> Projects { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<NumericPartResult> NumericPartResults { get; set; }
        public DbSet<CategoricalPartResult> CategoricalPartResults { get; set; }
        public DbSet<CategoricalZoneResult> CategoricalZoneResults { get; set; }

        // QualiSight entities
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<Defect> Defects { get; set; }
        public DbSet<DefectType> DefectTypes { get; set; }
        public DbSet<DefectField> DefectFields { get; set; }
        public DbSet<Disposition> Dispositions { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<PhotoDefect> PhotoDefects { get; set; }
        public DbSet<VisualSystemConfig> VisualSystemConfigs { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public IRSGeneratorDbContext(DbContextOptions<IRSGeneratorDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());

            modelBuilder.ApplyConfiguration(new IRSProjectConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterConfiguration());
            modelBuilder.ApplyConfiguration(new NumericPartResultConfiguration());
            modelBuilder.ApplyConfiguration(new CategoricalPartResultConfiguration());
            modelBuilder.ApplyConfiguration(new CategoricalZoneResultConfiguration());

            // QualiSight model configuration
            modelBuilder.Entity<PhotoDefect>().HasKey(pd => new { pd.PhotoId, pd.DefectId });

            modelBuilder.Entity<Defect>()
                .HasOne(d => d.OriginDefect)
                .WithMany(d => d.ChildDefects)
                .HasForeignKey(d => d.OriginDefectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.IrsProject)
                .WithMany(p => p.Inspections)
                .HasForeignKey(i => i.IrsProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.Inspector)
                .WithMany()
                .HasForeignKey(i => i.InspectorId)
                .OnDelete(DeleteBehavior.SetNull);

            SeedData(modelBuilder);
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            AddUser();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            AddUser();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow;

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedAt = now;
                }
                ((BaseEntity)entity.Entity).UpdatedAt = now;
            }
        }

        private void AddUser()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            var currentUser = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            User? user = Users.FirstOrDefault(u => u.WindowsAccount == currentUser);
            if (user != null)
            {
                foreach (var entity in entities)
                {
                    if (entity.State == EntityState.Added)
                    {
                        ((BaseEntity)entity.Entity).CreatedById = user.Id;
                    }
                    ((BaseEntity)entity.Entity).UpdatedById = user.Id;
                }
            }
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            var now = DateTime.UtcNow;

            // Users
            User user1 = new() { Id = 1, EmployeeId = "6518", FirstName = "Erdem", LastName = "Demirtaş", WindowsAccount = "TEIDOM\\k6518", DisplayName = "Erdem.Demirtaş", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            User user2 = new() { Id = 2, EmployeeId = "5956", FirstName = "Uras", LastName = "Erken", WindowsAccount = "TEIDOM\\k5956", DisplayName = "Uras.Erken", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };

            // Roles
            Role adminRole = new() { Id = 1, Name = Constants.Role.Admin, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            Role userWriteRole = new() { Id = 2, Name = Constants.Role.UserWriter, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            Role userReadRole = new() { Id = 3, Name = Constants.Role.UserReader, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };

            // Permissions - write
            List<Permission> entityWritePermissionList = new()
            {
                new() { Id = 1, Code = Constants.Claim.Value.IRSProjectWrite, Description = "[IRSProject] write permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 2, Code = Constants.Claim.Value.CharacterWrite, Description = "[Character] write permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 3, Code = Constants.Claim.Value.CategoricalPartResultWrite, Description = "[CategoricalPartResult] write permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 4, Code = Constants.Claim.Value.CategoricalZoneResultWrite, Description = "[CategoricalZoneResult] write permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 5, Code = Constants.Claim.Value.NumericPartResultWrite, Description = "[NumericPartResult] write permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
            };

            // Permissions - read
            List<Permission> entityReadPermissionList = new()
            {
                new() { Id = 6, Code = Constants.Claim.Value.IRSProjectRead, Description = "[IRSProject] read permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 7, Code = Constants.Claim.Value.CharacterRead, Description = "[Character] read permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 8, Code = Constants.Claim.Value.CategoricalPartResultRead, Description = "[CategoricalPartResult] read permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 9, Code = Constants.Claim.Value.CategoricalZoneResultRead, Description = "[CategoricalZoneResult] read permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 10, Code = Constants.Claim.Value.NumericPartResultRead, Description = "[NumericPartResult] read permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
            };

            Permission authorizationWritePermission = new() { Id = 11, Code = Constants.Claim.Value.AuthorizationWrite, Description = "Authorization write permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            Permission authorizationReadPermission = new() { Id = 12, Code = Constants.Claim.Value.AuthorizationRead, Description = "Authorization read permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };

            // UserRoles
            UserRole user1AdminRole = new() { Id = 1, UserId = 1, RoleId = 1, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            UserRole user2AdminRole = new() { Id = 2, UserId = 2, RoleId = 1, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            UserRole user1UserWriteRole = new() { Id = 3, UserId = 1, RoleId = 2, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            UserRole user2UserWriteRole = new() { Id = 4, UserId = 2, RoleId = 2, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            UserRole user1UserReadRole = new() { Id = 5, UserId = 1, RoleId = 3, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            UserRole user2UserReadRole = new() { Id = 6, UserId = 2, RoleId = 3, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };

            // RolePermissions
            long rolePermissionIdCounter = 1;
            List<RolePermission> adminRolePermissions = entityWritePermissionList
                .Select(p => new RolePermission { Id = rolePermissionIdCounter++, RoleId = 1, PermissionId = p.Id, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 })
                .Concat(new List<RolePermission>
                {
                    new RolePermission { Id = rolePermissionIdCounter++, RoleId = 1, PermissionId = 11, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                    new RolePermission { Id = rolePermissionIdCounter++, RoleId = 1, PermissionId = 12, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                })
                .ToList();

            List<RolePermission> userWriteRolePermissions = entityWritePermissionList
                .Select(p => new RolePermission { Id = rolePermissionIdCounter++, RoleId = 2, PermissionId = p.Id, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 })
                .ToList();

            List<RolePermission> userReadRolePermissions = entityReadPermissionList
                .Select(p => new RolePermission { Id = rolePermissionIdCounter++, RoleId = 3, PermissionId = p.Id, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 })
                .ToList();

            modelBuilder.Entity<User>().HasData(user1, user2);
            modelBuilder.Entity<Role>().HasData(adminRole, userWriteRole, userReadRole);
            modelBuilder.Entity<Permission>().HasData(
                entityWritePermissionList
                    .Concat(entityReadPermissionList)
                    .Concat(new List<Permission> { authorizationWritePermission, authorizationReadPermission })
                    .ToList());
            modelBuilder.Entity<UserRole>().HasData(user1AdminRole, user2AdminRole, user1UserWriteRole, user2UserWriteRole, user1UserReadRole, user2UserReadRole);
            modelBuilder.Entity<RolePermission>().HasData(
                adminRolePermissions
                    .Concat(userWriteRolePermissions)
                    .Concat(userReadRolePermissions)
                    .ToList());
        }
    }
}
