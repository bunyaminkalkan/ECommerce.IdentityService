using ECommerce.IdentityService.API.UseCases.Commands;
using Microsoft.AspNetCore.Mvc;
using Space.Abstraction;

namespace ECommerce.IdentityService.API.Controllers;

[ApiController]
[Route("[controller]/")]
public class AuthController(ISpace space) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        await space.Send(request);
        return Ok(new { message = "Registration successful! You can log in now" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        var response = await space.Send(request);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand request)
    {
        var response = await space.Send(request);
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand request)
    {
        var response = await space.Send(request);
        return Ok(response);
    }

    [HttpPost("revoke-all-tokens")]
    public async Task<IActionResult> RevokeAllTokens([FromBody] RevokeAllTokensCommand request)
    {
        var response = await space.Send(request);
        return Ok(response);
    }
}
