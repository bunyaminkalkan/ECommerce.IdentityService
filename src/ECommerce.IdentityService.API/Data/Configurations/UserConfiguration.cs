using ECommerce.IdentityService.API.Constants;
using ECommerce.IdentityService.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.IdentityService.API.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(Tables.Users);

        //IdentityUser zaten tanımlıyor
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired(false);
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired(false);
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired(false);
        builder.Property(u => u.IsDeleted).IsRequired();
        builder.Property(u => u.DeletedAt).IsRequired(false);
    }
}