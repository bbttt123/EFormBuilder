using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EFormBuilder.Domain.Entities;

namespace EFormBuilder.Infrastructure.Persistence.Configurations;

public class FieldConfiguration : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Label)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.FieldType)
            .IsRequired();

        builder.HasMany(x => x.FieldOptions)
            .WithOne(x => x.Field)
            .HasForeignKey(x => x.FieldId);
    }
}