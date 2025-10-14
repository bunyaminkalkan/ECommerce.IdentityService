using ECommerce.IdentityService.API.Constants;
using ECommerce.IdentityService.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.IdentityService.API.Data.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(Tables.Roles);

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name).HasMaxLength(256).IsRequired();
        builder.Property(r => r.NormalizedName).HasMaxLength(256).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.UpdatedAt).IsRequired(false);
        builder.Property(r => r.IsDeleted).IsRequired();
        builder.Property(r => r.DeletedAt).IsRequired(false);
    }
}
