using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EFormBuilder.Domain.Entities;

namespace EFormBuilder.Infrastructure.Persistence.Configurations;

public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasMany(x => x.Fields)
            .WithOne(x => x.Form)
            .HasForeignKey(x => x.FormId);

        builder.HasMany(x => x.Responses)
            .WithOne(x => x.Form)
            .HasForeignKey(x => x.FormId);
    }
}