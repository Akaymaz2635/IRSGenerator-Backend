using Microsoft.EntityFrameworkCore;
using IRSGenerator.Core.Entities;

namespace IRSGenerator.Data;

public class IRSGeneratorDbContext : DbContext
{
    public IRSGeneratorDbContext(DbContextOptions<IRSGeneratorDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<Defect> Defects => Set<Defect>();
    public DbSet<DefectType> DefectTypes => Set<DefectType>();
    public DbSet<DefectField> DefectFields => Set<DefectField>();
    public DbSet<Disposition> Dispositions => Set<Disposition>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<PhotoDefect> PhotoDefects => Set<PhotoDefect>();
    public DbSet<VisualSystemConfig> VisualSystemConfigs => Set<VisualSystemConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PhotoDefect composite key
        modelBuilder.Entity<PhotoDefect>()
            .HasKey(pd => new { pd.PhotoId, pd.DefectId });

        modelBuilder.Entity<PhotoDefect>()
            .HasOne(pd => pd.Photo)
            .WithMany(p => p.PhotoDefects)
            .HasForeignKey(pd => pd.PhotoId);

        modelBuilder.Entity<PhotoDefect>()
            .HasOne(pd => pd.Defect)
            .WithMany(d => d.PhotoDefects)
            .HasForeignKey(pd => pd.DefectId);

        // Self-referential Defect
        modelBuilder.Entity<Defect>()
            .HasOne(d => d.OriginDefect)
            .WithMany(d => d.ChildDefects)
            .HasForeignKey(d => d.OriginDefectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Defect -> Inspection (restrict to avoid cascade conflict)
        modelBuilder.Entity<Defect>()
            .HasOne(d => d.Inspection)
            .WithMany(i => i.Defects)
            .HasForeignKey(d => d.InspectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Photo -> Inspection
        modelBuilder.Entity<Photo>()
            .HasOne(p => p.Inspection)
            .WithMany(i => i.Photos)
            .HasForeignKey(p => p.InspectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Disposition -> Defect
        modelBuilder.Entity<Disposition>()
            .HasOne(d => d.Defect)
            .WithMany(d => d.Dispositions)
            .HasForeignKey(d => d.DefectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
