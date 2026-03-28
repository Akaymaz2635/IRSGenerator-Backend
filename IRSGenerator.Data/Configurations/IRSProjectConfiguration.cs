using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRSGenerator.Data.Configurations;

internal class IRSProjectConfiguration : BaseEntityConfiguration<IRSProject>
{
    public override void Configure(EntityTypeBuilder<IRSProject> builder)
    {
        base.Configure(builder);

        builder.ToTable("IRSProjects");

        builder.Property(e => e.ProjectType).IsRequired();
        builder.Property(e => e.PartNumber).IsRequired();
        builder.Property(e => e.SerialNumber).IsRequired();

        builder.HasOne(e => e.Owner)
            .WithMany()
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
