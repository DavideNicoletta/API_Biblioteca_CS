using BibliotecaApi.Dtos;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

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
    public async Task<ActionResult<ResponseApi<string>>> Login(LoginRequestDto request)
    {
        string? token = await _authService.LoginAsync(request);
        if(token == null)
        {
            return Unauthorized();
        }

        return Ok(ResponseApi<string>.Success(token));
    }
}