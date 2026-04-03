using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EFormBuilder.Domain.Entities;

namespace EFormBuilder.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(500); // Token thường dài nên để thoải mái tí

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        // Quan hệ: Một User có nhiều RefreshToken
        builder.HasOne(x => x.User)
            .WithMany() // Nếu bên User.cs bạn không khai báo ICollection<RefreshToken> thì để trống thế này
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa User thì xóa luôn Token của họ

        // Index để tìm kiếm Token cho nhanh (vì sau này hàm Refresh sẽ gọi thường xuyên)
        builder.HasIndex(x => x.Token).IsUnique();
    }
}