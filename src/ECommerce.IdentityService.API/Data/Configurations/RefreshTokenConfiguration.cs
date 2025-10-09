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

        builder.Property(r => r.Token).HasMaxLength(200);

        builder.HasIndex(r => r.Token).IsUnique();

        builder.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId);

        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.UpdatedAt).IsRequired(false);
        builder.Property(r => r.IsDeleted).IsRequired();
        builder.Property(r => r.DeletedAt).IsRequired(false);
    }
}
