using Space.Abstraction;
using Space.Abstraction.Contracts;

namespace ECommerce.IdentityService.API.UseCases.Commands;

public sealed record RevokeAllTokensCommand(Guid UserId) : IRequest<Nothing>;
