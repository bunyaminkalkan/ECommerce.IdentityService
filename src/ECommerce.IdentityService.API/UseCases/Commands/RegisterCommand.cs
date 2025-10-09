using Space.Abstraction;
using Space.Abstraction.Contracts;

namespace ECommerce.IdentityService.API.UseCases.Commands;

public sealed record RegisterCommand(
    string? FirstName,
    string? LastName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword
    ) : IRequest<Nothing>;

