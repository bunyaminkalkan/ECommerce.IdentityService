using ECommerce.IdentityService.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.IdentityService.API.Data.Context;

public class AppDbContext : IdentityUserContext<User, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    //public virtual DbSet<User> Users { get; set; } zaten IdentityDbContext içinde var
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Tablo adlarını ve şemayı özelleştir
        builder.Entity<IdentityUserClaim<Guid>>(e => { e.ToTable(name: "user_claims"); });
        builder.Entity<IdentityUserLogin<Guid>>(e => { e.ToTable(name: "user_logins"); });
        builder.Entity<IdentityUserToken<Guid>>(e => { e.ToTable(name: "user_tokens"); });

        builder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        builder.Entity<RefreshToken>().HasQueryFilter(r => !r.IsDeleted);

        // Konfigürasyonları uygula
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}