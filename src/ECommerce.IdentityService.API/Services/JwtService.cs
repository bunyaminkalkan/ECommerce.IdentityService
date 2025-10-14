using ECommerce.BuildingBlocks.Shared.Kernel.Auth.Options;
using ECommerce.IdentityService.API.Domain.Entities;
using ECommerce.IdentityService.API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.IdentityService.API.Services;

public sealed class JwtService : IJwtService
{
    private readonly JwtOptions _jwtOptions;
    private readonly UserManager<User> _userManager;

    public JwtService(IOptions<JwtOptions> jwtOptions, UserManager<User> userManager)
    {
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
    }

    public async Task<TokenDTO> CreateTokenAsync(User user)
    {
        var expires = DateTime.UtcNow.Add(TimeSpan.FromSeconds(_jwtOptions.Expiration));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Kullanıcının rollerini al ve claim olarak ekle
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Kullanıcının claim'lerini al ve ekle
        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256));

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var refreshToken = GenerateRefreshToken();

        return new TokenDTO
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpirationDate = expires
        };
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}