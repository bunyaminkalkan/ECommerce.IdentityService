using Space.Abstraction;
using Space.Abstraction.Contracts;

namespace ECommerce.IdentityService.API.UseCases.Commands;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Nothing>;
