﻿using ECommerce.IdentityService.API.Domain.Entities;
using ECommerce.IdentityService.API.DTOs;
using ECommerce.IdentityService.API.Options;
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

    public JwtService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<TokenDTO> CreateTokenAsync(User user)
    {
        var expires = DateTime.Now.Add(TimeSpan.FromSeconds(_jwtOptions.Expiration));

        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Exp, expires.ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
            new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.Audience)
        };

        JwtSecurityToken jwtSecurityToken = new(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtOptions.SecretKey))
                , SecurityAlgorithms.HmacSha256));

        string token = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        return new TokenDTO
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpirationDate = expires
        };
    }
}
