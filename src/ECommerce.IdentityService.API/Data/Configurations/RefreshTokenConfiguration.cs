using ECommerce.IdentityService.API.Constants;
using ECommerce.IdentityService.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ECommerce.IdentityService.API.Data.Configurations;
public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable(Tables.RefreshTokens);

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Token)
               .IsRequired()
               .HasMaxLength(200);

        builder.HasIndex(r => r.Token).IsUnique();

        builder.HasOne(r => r.User)
               .WithMany()
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.Property(r => r.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.Expires)
               .IsRequired();

        builder.Property(r => r.IsRevoked)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(r => r.RevokedAt)
               .IsRequired(false);

        builder.Property(r => r.IsDeleted)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(r => r.DeletedAt)
               .IsRequired(false);
    }
}
