namespace MiniSupermarketSystem.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using MiniSupermarketSystem.Domain.Interfaces.IServices;



[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        
        var token = await _authService.Authenticate(request.Username, request.Password);

        if (token == null)
            return Unauthorized();

        return Ok(new { Token = token });
    }
}

public record LoginRequest(string Username, string Password);
