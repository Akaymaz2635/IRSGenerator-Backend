using MES.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MES.Infrastructure.Persistence.Configurations;

internal class NumericPartResultConfiguration : BaseEntityConfiguration<NumericPartResult>
{
    public override void Configure(EntityTypeBuilder<NumericPartResult> builder)
    {
        base.Configure(builder);
        builder.ToTable("NumericPartResults");
    }
}
