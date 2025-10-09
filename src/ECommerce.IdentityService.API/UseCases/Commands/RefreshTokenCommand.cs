using ECommerce.IdentityService.API.DTOs;
using Space.Abstraction.Contracts;

namespace ECommerce.IdentityService.API.UseCases.Commands;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<TokenDTO>;
