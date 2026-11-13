using BibliotecaApi.Dtos;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<ResponseApi<UserResponseDto>>> Create(UserRequestDto request)
    {
        var created = await _userService.CreateAsync(request);
        if (created == null)
        {
            return Conflict();
        }

        return CreatedAtAction(nameof(GetByUuid), new { uuid = created.Uuid }, ResponseApi<UserResponseDto>.Success(created));
    }

    [HttpGet]
    public async Task<ActionResult<ResponseApi<List<UserResponseDto>>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(ResponseApi<List<UserResponseDto>>.Success(users));
    }

    [HttpGet("{uuid}")]
    public async Task<ActionResult<ResponseApi<UserResponseDto>>> GetByUuid(string uuid)
    {
        var user = await _userService.GetByUuidAsync(uuid);
        if(user == null)
        {
            return NotFound();
        }

        return Ok(ResponseApi<UserResponseDto>.Success(user));
    }
}