using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRSGenerator.Data.Configurations;

internal class CategoricalZoneResultConfiguration : BaseEntityConfiguration<CategoricalZoneResult>
{
    public override void Configure(EntityTypeBuilder<CategoricalZoneResult> builder)
    {
        base.Configure(builder);
        builder.ToTable("CategoricalZoneResults");
    }
}
