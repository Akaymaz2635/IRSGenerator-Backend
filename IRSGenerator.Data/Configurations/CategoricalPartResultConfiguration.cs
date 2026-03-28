using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IRSGenerator.Data.Configurations;

internal class CategoricalPartResultConfiguration : BaseEntityConfiguration<CategoricalPartResult>
{
    public override void Configure(EntityTypeBuilder<CategoricalPartResult> builder)
    {
        base.Configure(builder);
        builder.ToTable("CategoricalPartResults");
    }
}
