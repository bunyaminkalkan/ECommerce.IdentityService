using ECommerce.IdentityService.API.Constants;
using ECommerce.IdentityService.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.IdentityService.API.Data.Context;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Identity tablolarını özelleştir
        builder.Entity<IdentityUserClaim<Guid>>(e => e.ToTable(Tables.UserClaims));
        builder.Entity<IdentityUserLogin<Guid>>(e => e.ToTable(Tables.UserLogins));
        builder.Entity<IdentityUserToken<Guid>>(e => e.ToTable(Tables.UserTokens));
        builder.Entity<IdentityUserRole<Guid>>(e => e.ToTable(Tables.UserRoles));
        builder.Entity<IdentityRoleClaim<Guid>>(e => e.ToTable(Tables.RoleClaims));

        // Global query filters
        builder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        builder.Entity<Role>().HasQueryFilter(r => !r.IsDeleted);
        builder.Entity<RefreshToken>().HasQueryFilter(r => !r.IsDeleted);

        // Konfigürasyonları uygula (User ve Role tablo isimleri burada ayarlanıyor)
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}