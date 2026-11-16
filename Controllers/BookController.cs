using BibliotecaApi.Dtos;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BibliotecaApi.Controllers;


[ApiController]
[Route("api/book")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService){
        _bookService = bookService;
    }


    [HttpGet]
    public async Task<ActionResult<ResponseApi<List<BookResponseDto>>>> GetAll()
    {
        var books = await _bookService.GetAllAsync();
        return Ok(ResponseApi<List<BookResponseDto>>.Success(books));
    }

    [HttpGet("uuid/{uuid}")]
    public async Task<ActionResult<ResponseApi<BookResponseDto>>> GetByUuid(string uuid)
    {
        var book = await _bookService.GetByUuidAsync(uuid);
        if(book == null)
        {
            return NotFound();
        }

        return Ok(ResponseApi<BookResponseDto>.Success(book));
    }

    [HttpGet("isbn/{isbn}")]
    public async Task<ActionResult<ResponseApi<BookResponseDto>>> GetByIsbn(string isbn)
    {
        var book = await _bookService.GetByIsbnAsync(isbn);
        if(book == null)
        {
            return NotFound();
        }

        return Ok(ResponseApi<BookResponseDto>.Success(book));
    }

    
    [HttpPost]
    [Authorize(Roles = "ROLE_ADMIN")]
    public async Task<ActionResult<ResponseApi<BookResponseDto>>> Create(BookRequestDto request)
    {
        var created = await _bookService.CreateAsync(request);
        if(created == null)
        {
            return Conflict();
        }

        return CreatedAtAction(nameof(GetByUuid), new {uuid = created.Uuid}, ResponseApi<BookResponseDto>.Success(created));
    }


    [HttpPut("{uuid}")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public async Task<ActionResult<ResponseApi<BookResponseDto>>> Update(string uuid, BookRequestDto request)
    {
        var (result, book) = await _bookService.UpdateAsync(uuid, request);

        if(result == UpdateResult.NotFound)
        {
            return NotFound();
        }

        if(result == UpdateResult.IsbnConflict)
        {
            return Conflict();
        }

        return Ok(ResponseApi<BookResponseDto>.Success(book!));
    }


    [HttpDelete("{uuid}")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public async Task<IActionResult> Delete(string uuid)
    {
        bool delete = await _bookService.DeleteAsync(uuid);
        if(!delete)
        {
            return NotFound();
        }

        return NoContent();
    } 
}