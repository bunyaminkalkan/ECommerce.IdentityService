using ECommerce.IdentityService.API.UseCases.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    [EnableRateLimiting("login")]
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
}
