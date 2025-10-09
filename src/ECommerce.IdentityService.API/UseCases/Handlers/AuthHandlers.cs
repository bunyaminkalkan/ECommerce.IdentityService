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

namespace ECommerce.IdentityService.API.UseCases.Handlers;

public class AuthHandlers(IJwtService jwtService, UserManager<User> userManager, AppDbContext appDbContext)
{
    [Handle]
    public async Task<TokenDTO> Login(HandlerContext<LoginCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByEmailAsync(ctx.Request.Email)
            ?? throw new BadRequestException("Invalid email or password");

        var isValidPassword = await userManager.CheckPasswordAsync(user, ctx.Request.Password);

        if (!isValidPassword)
            throw new BadRequestException("Invalid email or password");

        // Token oluştur ve refresh token kaydet
        var tokens = await jwtService.CreateTokenAsync(user);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = tokens.RefreshToken
        };

        appDbContext.RefreshTokens.Add(refreshToken);
        await appDbContext.SaveChangesAsync(ctx.CancellationToken);

        return new TokenDTO
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = refreshToken.Token,
            ExpirationDate = tokens.ExpirationDate
        };
    }

    [Handle]
    public async Task<Nothing> Register(HandlerContext<RegisterCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = ctx.Request.UserName,
            Email = ctx.Request.Email,
            FirstName = ctx.Request.FirstName,
            LastName = ctx.Request.LastName,
            EmailConfirmed = false,
        };

        var result = await userManager.CreateAsync(user, ctx.Request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Register failed: {errors}");
        }

        return Nothing.Task.Result;
    }

    [Handle]
    public async Task<TokenDTO> RefreshToken(HandlerContext<RefreshTokenCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var refreshToken = await appDbContext.RefreshTokens.Where(r => r.Token == ctx.Request.RefreshToken).Include(r => r.User).FirstOrDefaultAsync()
            ?? throw new BadRequestException("Enter a valid refresh token");

        var newTokens = await jwtService.CreateTokenAsync(refreshToken.User);

        refreshToken.Token = newTokens.RefreshToken;
        refreshToken.Expires = DateTime.UtcNow.AddMonths(1);

        appDbContext.RefreshTokens.Update(refreshToken);
        await appDbContext.SaveChangesAsync();

        return new TokenDTO
        {
            AccessToken = newTokens.AccessToken,
            RefreshToken = newTokens.RefreshToken,
            ExpirationDate = newTokens.ExpirationDate,
        };

    }
}
