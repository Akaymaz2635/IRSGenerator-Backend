using MES.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MES.Infrastructure.Persistence.Configurations;

internal class CategoricalZoneResultConfiguration : BaseEntityConfiguration<CategoricalZoneResult>
{
    public override void Configure(EntityTypeBuilder<CategoricalZoneResult> builder)
    {
        base.Configure(builder);
        builder.ToTable("CategoricalZoneResults");
    }
}
