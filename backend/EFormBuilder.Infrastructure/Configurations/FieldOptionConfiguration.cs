using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EFormBuilder.Domain.Entities;

namespace EFormBuilder.Infrastructure.Persistence.Configurations;

public class FieldOptionConfiguration : IEntityTypeConfiguration<FieldOption>
{
    public void Configure(EntityTypeBuilder<FieldOption> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
            .IsRequired();
    }
}