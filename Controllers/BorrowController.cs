using BibliotecaApi.Dtos;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/borrow")]
[Authorize]
public class BorrowController : ControllerBase
{
    private readonly IBorrowService _borrowService;

    public BorrowController(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    private string CurrentUserUuid => User.FindFirst("sub")?.Value ?? string.Empty;
    private bool IsAdmin => User.IsInRole("ROLE_ADMIN");

    [HttpGet]
    public async Task<ActionResult<ResponseApi<List<BorrowResponseDto>>>> GetAll()
    {
        var borrows = await _borrowService.GetAllAsync(CurrentUserUuid, IsAdmin);
        return Ok(ResponseApi<List<BorrowResponseDto>>.Success(borrows));
    }

    [HttpGet("{uuid}")]
    public async Task<ActionResult<ResponseApi<BorrowResponseDto>>> GetByUuid(string uuid)
    {
        var borrow = await _borrowService.GetByUuidAsync(uuid, CurrentUserUuid, IsAdmin);
        if(borrow == null)
        {
            return NotFound();
        }

        return Ok(ResponseApi<BorrowResponseDto>.Success(borrow));
    }

    [HttpPost]
    public async Task<ActionResult<ResponseApi<BorrowResponseDto>>> Create(BorrowRequestDto request)
    {
        var (result, borrow) = await _borrowService.CreateAsync(CurrentUserUuid, request);

        if (result.Equals(BorrowResult.NotFound))
        {
            return NotFound();
        }
        if (result.Equals(BorrowResult.NoCopiesAvailable))
        {
            return Conflict();
        }

        return CreatedAtAction(nameof(GetByUuid), new { uuid = borrow!.Uuid }, ResponseApi<BorrowResponseDto>.Success(borrow));
    }

    [HttpPut("{uuid}/return")]
    public async Task<ActionResult<ResponseApi<BorrowResponseDto>>> Return(string uuid)
    {
        var (result, borrow) = await _borrowService.ReturnAsync(uuid, CurrentUserUuid, IsAdmin);
        if(result.Equals(BorrowResult.NotFound))
        {
            return NotFound();
        }
        
        if(result.Equals(BorrowResult.AlreadyReturned))
        {
            return Conflict();
        }

        return Ok(ResponseApi<BorrowResponseDto>.Success(borrow!));

    }

    [HttpDelete("{uuid}")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public async Task<IActionResult> Delete(string uuid)
    {
        bool delete = await _borrowService.DeleteAsync(uuid);
        if(!delete)
        {
            return NotFound();
        }

        return NoContent();
    }
}