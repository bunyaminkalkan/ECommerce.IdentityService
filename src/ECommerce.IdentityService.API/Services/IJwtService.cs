using ECommerce.IdentityService.API.Domain.Entities;
using ECommerce.IdentityService.API.DTOs;

namespace ECommerce.IdentityService.API.Services;

public interface IJwtService
{
    Task<TokenDTO> CreateTokenAsync(User user);
}
