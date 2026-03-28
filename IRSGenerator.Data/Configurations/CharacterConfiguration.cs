using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRSGenerator.Data.Configurations;

internal class CharacterConfiguration : BaseEntityConfiguration<Character>
{
    public override void Configure(EntityTypeBuilder<Character> builder)
    {
        base.Configure(builder);

        builder.ToTable("Characters");

        builder.Property(e => e.ItemNo).IsRequired();
        builder.Property(e => e.Dimension).IsRequired();
        builder.Property(e => e.InspectionResult).HasDefaultValue("Unidentified");

        builder.HasOne(e => e.IRSProject)
            .WithMany(p => p.Characters)
            .HasForeignKey(e => e.IRSProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.NumericPartResults)
            .WithOne(r => r.Character)
            .HasForeignKey(r => r.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.CategoricalPartResults)
            .WithOne(r => r.Character)
            .HasForeignKey(r => r.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.CategoricalZoneResults)
            .WithOne(r => r.Character)
            .HasForeignKey(r => r.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
