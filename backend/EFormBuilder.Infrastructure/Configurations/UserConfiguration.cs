using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EFormBuilder.Domain.Entities;

namespace EFormBuilder.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.HasMany(x => x.Forms)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}