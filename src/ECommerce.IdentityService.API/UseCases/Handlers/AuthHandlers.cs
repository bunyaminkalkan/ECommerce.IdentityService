using ECommerce.BuildingBlocks.Shared.Kernel.Exceptions;
using ECommerce.IdentityService.API.Data.Context;
using ECommerce.IdentityService.API.Domain.Entities;
using ECommerce.IdentityService.API.DTOs;
using ECommerce.IdentityService.API.Services;
using ECommerce.IdentityService.API.UseCases.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Space.Abstraction;
using Space.Abstraction.Attributes;
using Space.Abstraction.Context;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.IdentityService.API.UseCases.Handlers;

public class AuthHandlers(
    IJwtService jwtService,
    UserManager<User> userManager,
    AppDbContext appDbContext)
{
    [Handle]
    public async Task<TokenDTO> Login(HandlerContext<LoginCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.Users
            .Where(u => u.Email == ctx.Request.Email)
            .FirstOrDefaultAsync(ctx.CancellationToken)
            ?? throw new BadRequestException("Invalid email or password");

        var isValidPassword = await userManager.CheckPasswordAsync(user, ctx.Request.Password);
        if (!isValidPassword)
            throw new BadRequestException("Invalid email or password");

        // Lockout kontrolü
        if (await userManager.IsLockedOutAsync(user))
            throw new BadRequestException("Account is locked. Please try again later.");

        var tokens = await jwtService.CreateTokenAsync(user);

        var hashedRefreshToken = HashToken(tokens.RefreshToken);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = hashedRefreshToken,
            Expires = DateTime.UtcNow.AddMonths(1),
            CreatedAt = DateTime.UtcNow
        };

        appDbContext.RefreshTokens.Add(refreshToken);

        await userManager.ResetAccessFailedCountAsync(user);

        await appDbContext.SaveChangesAsync(ctx.CancellationToken);

        return new TokenDTO
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            ExpirationDate = tokens.ExpirationDate
        };
    }

    [Handle]
    public async Task<Nothing> Register(HandlerContext<RegisterCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var existingUser = await userManager.FindByEmailAsync(ctx.Request.Email);
        if (existingUser != null)
            throw new BadRequestException("Email already exists");

        if (!string.IsNullOrEmpty(ctx.Request.UserName))
        {
            var existingUsername = await userManager.FindByNameAsync(ctx.Request.UserName);
            if (existingUsername != null)
                throw new BadRequestException("Username already exists");
        }

        var user = new User
        {
            UserName = ctx.Request.UserName ?? ctx.Request.Email,
            Email = ctx.Request.Email,
            FirstName = ctx.Request.FirstName,
            LastName = ctx.Request.LastName,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, ctx.Request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Registration failed: {errors}");
        }

        await userManager.AddToRoleAsync(user, "Customer");

        return Nothing.Task.Result;
    }

    [Handle]
    public async Task<TokenDTO> RefreshToken(HandlerContext<RefreshTokenCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var hashedToken = HashToken(ctx.Request.RefreshToken);

        var refreshToken = await appDbContext.RefreshTokens
            .Include(r => r.User)
            .Where(r => r.Token == hashedToken &&
                       !r.IsRevoked &&
                       r.Expires > DateTime.UtcNow)
            .FirstOrDefaultAsync(ctx.CancellationToken)
            ?? throw new BadRequestException("Invalid or expired refresh token");

        if (refreshToken.User.IsDeleted)
            throw new BadRequestException("User account has been deleted");

        var newTokens = await jwtService.CreateTokenAsync(refreshToken.User);

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;

        var newRefreshToken = new RefreshToken
        {
            UserId = refreshToken.UserId,
            Token = HashToken(newTokens.RefreshToken),
            Expires = DateTime.UtcNow.AddMonths(1),
            CreatedAt = DateTime.UtcNow
        };

        appDbContext.RefreshTokens.Add(newRefreshToken);
        await appDbContext.SaveChangesAsync(ctx.CancellationToken);

        return new TokenDTO
        {
            AccessToken = newTokens.AccessToken,
            RefreshToken = newTokens.RefreshToken,
            ExpirationDate = newTokens.ExpirationDate
        };
    }

    [Handle]
    public async Task<Nothing> Logout(HandlerContext<LogoutCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var hashedToken = HashToken(ctx.Request.RefreshToken);

        var refreshToken = await appDbContext.RefreshTokens
            .Where(r => r.Token == hashedToken)
            .FirstOrDefaultAsync(ctx.CancellationToken);

        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            await appDbContext.SaveChangesAsync(ctx.CancellationToken);
        }

        return Nothing.Task.Result;
    }

    [Handle]
    public async Task<Nothing> RevokeAllTokens(HandlerContext<RevokeAllTokensCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var tokens = await appDbContext.RefreshTokens
            .Where(r => r.UserId == ctx.Request.UserId && !r.IsRevoked)
            .ToListAsync(ctx.CancellationToken);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        await appDbContext.SaveChangesAsync(ctx.CancellationToken);

        return Nothing.Task.Result;
    }

    private static string HashToken(string token)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
    }
}