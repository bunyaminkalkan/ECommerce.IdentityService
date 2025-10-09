using ECommerce.IdentityService.API.DTOs;
using Space.Abstraction.Contracts;

namespace ECommerce.IdentityService.API.UseCases.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<TokenDTO>;
