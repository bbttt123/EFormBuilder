using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EFormBuilder.Domain.Entities;

namespace EFormBuilder.Infrastructure.Persistence.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AnswerText);

        builder.HasOne(x => x.Field)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.FieldId);
    }
}