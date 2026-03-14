using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EFormBuilder.Domain.Entities;

namespace EFormBuilder.Infrastructure.Persistence.Configurations;

public class ResponseConfiguration : IEntityTypeConfiguration<Response>
{
    public void Configure(EntityTypeBuilder<Response> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResponderEmail)
            .HasMaxLength(255);

        builder.HasMany(x => x.Answers)
            .WithOne(x => x.Response)
            .HasForeignKey(x => x.ResponseId);
    }
}