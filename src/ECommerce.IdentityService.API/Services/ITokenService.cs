namespace ECommerce.IdentityService.API.Services;
public interface ITokenService
{
    string GenerateToken();
    string HashToken(string token);
    bool ValidateToken(string token, string hashedToken);
}
