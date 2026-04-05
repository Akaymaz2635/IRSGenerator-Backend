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
        public DbSet<VisualProject> VisualProjects { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<Defect> Defects { get; set; }
        public DbSet<DefectType> DefectTypes { get; set; }
        public DbSet<DefectField> DefectFields { get; set; }
        public DbSet<Disposition> Dispositions { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<PhotoDefect> PhotoDefects { get; set; }
        public DbSet<VisualSystemConfig> VisualSystemConfigs { get; set; }
        public DbSet<DispositionType> DispositionTypes { get; set; }
        public DbSet<DispositionTransition> DispositionTransitions { get; set; }

        // NCM entities
        public DbSet<CauseCode> CauseCodes { get; set; }
        public DbSet<NcmDispositionType> NcmDispositionTypes { get; set; }

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

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.VisualProject)
                .WithMany(p => p.Inspections)
                .HasForeignKey(i => i.VisualProjectId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.InspectorUser)
                .WithMany()
                .HasForeignKey(i => i.InspectorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Defect>()
                .HasOne(d => d.OriginDefect)
                .WithMany(d => d.ChildDefects)
                .HasForeignKey(d => d.OriginDefectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.IrsProject)
                .WithMany(p => p.Inspections)
                .HasForeignKey(i => i.IrsProjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // DispositionType — unique index on Code
            modelBuilder.Entity<DispositionType>()
                .HasIndex(dt => dt.Code)
                .IsUnique();

            // DispositionTransition — Code-based FKs (string, not Id-based)
            modelBuilder.Entity<DispositionTransition>()
                .HasOne(t => t.FromType)
                .WithMany(dt => dt.TransitionsFrom)
                .HasForeignKey(t => t.FromCode)
                .HasPrincipalKey(dt => dt.Code)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<DispositionTransition>()
                .HasOne(t => t.ToType)
                .WithMany(dt => dt.TransitionsTo)
                .HasForeignKey(t => t.ToCode)
                .HasPrincipalKey(dt => dt.Code)
                .OnDelete(DeleteBehavior.Restrict);

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
                new() { Id = 5, Code = Constants.Claim.Value.NumericalPartResultWrite, Description = "[NumericalPartResult] write permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
            };

            // Permissions - read
            List<Permission> entityReadPermissionList = new()
            {
                new() { Id = 6, Code = Constants.Claim.Value.IRSProjectRead, Description = "[IRSProject] read permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 7, Code = Constants.Claim.Value.CharacterRead, Description = "[Character] read permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 8, Code = Constants.Claim.Value.CategoricalPartResultRead, Description = "[CategoricalPartResult] read permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 9, Code = Constants.Claim.Value.CategoricalZoneResultRead, Description = "[CategoricalZoneResult] read permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
                new() { Id = 10, Code = Constants.Claim.Value.NumericalPartResultRead, Description = "[NumericalPartResult] read permission.", CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 },
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

            // ── DispositionType seed ──────────────────────────────────────────
            var dt1  = new DispositionType { Id =  1, Code = "USE_AS_IS",    Label = "Kabul (Spec)",                             CssClass = "disp-accepted",      IsNeutralizing = true,  IsInitial = true,  SortOrder =  1, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt2  = new DispositionType { Id =  2, Code = "KABUL_RESIM",  Label = "Kabul (Resim)",                            CssClass = "disp-accepted",      IsNeutralizing = true,  IsInitial = true,  SortOrder =  2, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt3  = new DispositionType { Id =  3, Code = "CONFORMS",     Label = "Uygun (Inspector)",                        CssClass = "disp-conforms",      IsNeutralizing = true,  IsInitial = false, SortOrder =  3, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt4  = new DispositionType { Id =  4, Code = "REWORK",       Label = "Rework",                                   CssClass = "disp-rework",        IsNeutralizing = false, IsInitial = true,  SortOrder =  4, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt5  = new DispositionType { Id =  5, Code = "RE_INSPECT",   Label = "Yeniden İnceleme",                         CssClass = "disp-re-inspect",    IsNeutralizing = false, IsInitial = true,  SortOrder =  5, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt6  = new DispositionType { Id =  6, Code = "CTP_RE_INSPECT", Label = "CTP — Sonraki Op. Yeniden İnceleme",     CssClass = "disp-mrb-ctp",       IsNeutralizing = false, IsInitial = true,  SortOrder =  6, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt7  = new DispositionType { Id =  7, Code = "MRB_SUBMITTED", Label = "MRB Gönderildi",                          CssClass = "disp-mrb-submitted", IsNeutralizing = false, IsInitial = true,  SortOrder =  7, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt8  = new DispositionType { Id =  8, Code = "MRB_CTP",      Label = "CTP — MRB (Devam)",                        CssClass = "disp-mrb-ctp",       IsNeutralizing = false, IsInitial = true,  SortOrder =  8, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt9  = new DispositionType { Id =  9, Code = "MRB_ACCEPTED", Label = "MRB Kabul",                                CssClass = "disp-mrb-accepted",  IsNeutralizing = true,  IsInitial = false, SortOrder =  9, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt10 = new DispositionType { Id = 10, Code = "MRB_REJECTED", Label = "MRB Ret",                                  CssClass = "disp-mrb-rejected",  IsNeutralizing = true,  IsInitial = false, SortOrder = 10, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt11 = new DispositionType { Id = 11, Code = "VOID",         Label = "Void",                                     CssClass = "disp-void",          IsNeutralizing = true,  IsInitial = true,  SortOrder = 11, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt12 = new DispositionType { Id = 12, Code = "REPAIR",       Label = "Repair",                                   CssClass = "disp-repair",        IsNeutralizing = true,  IsInitial = true,  SortOrder = 12, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };
            var dt13 = new DispositionType { Id = 13, Code = "SCRAP",        Label = "Scrap",                                    CssClass = "disp-scrap",         IsNeutralizing = true,  IsInitial = true,  SortOrder = 13, Active = true, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 };

            modelBuilder.Entity<DispositionType>().HasData(dt1, dt2, dt3, dt4, dt5, dt6, dt7, dt8, dt9, dt10, dt11, dt12, dt13);

            // ── DispositionTransition seed ────────────────────────────────────
            // null FromCode = initial (no prior decision)
            // FromCode matches allowedNextDecisions() logic in inspection-detail.js
            var transitions = new List<DispositionTransition>();
            long tid = 1;

            void AddTransition(string? from, string to) =>
                transitions.Add(new DispositionTransition { Id = tid++, FromCode = from, ToCode = to, CreatedAt = now, CreatedById = 1, UpdatedAt = now, UpdatedById = 1 });

            // Initial (null → FULL_DECISIONS)
            string[] fullDecisions = ["USE_AS_IS","KABUL_RESIM","REWORK","RE_INSPECT","CTP_RE_INSPECT","MRB_SUBMITTED","MRB_CTP","VOID","REPAIR","SCRAP"];
            foreach (var code in fullDecisions) AddTransition(null, code);

            // REWORK → CONFORMS + FULL_DECISIONS (re-inspect implicit after rework)
            AddTransition("REWORK", "CONFORMS");
            foreach (var code in fullDecisions) AddTransition("REWORK", code);

            // RE_INSPECT (bağımsız seçim olarak başlangıçtan seçilebilir) → CONFORMS + FULL_DECISIONS
            AddTransition("RE_INSPECT", "CONFORMS");
            foreach (var code in fullDecisions) AddTransition("RE_INSPECT", code);

            // CTP_RE_INSPECT → CONFORMS + FULL_DECISIONS
            AddTransition("CTP_RE_INSPECT", "CONFORMS");
            foreach (var code in fullDecisions) AddTransition("CTP_RE_INSPECT", code);

            // MRB_SUBMITTED → MRB_CTP | MRB_ACCEPTED | MRB_REJECTED
            AddTransition("MRB_SUBMITTED", "MRB_CTP");
            AddTransition("MRB_SUBMITTED", "MRB_ACCEPTED");
            AddTransition("MRB_SUBMITTED", "MRB_REJECTED");

            // MRB_CTP → MRB_ACCEPTED | MRB_REJECTED
            AddTransition("MRB_CTP", "MRB_ACCEPTED");
            AddTransition("MRB_CTP", "MRB_REJECTED");

            modelBuilder.Entity<DispositionTransition>().HasData(transitions);
        }
    }
}
