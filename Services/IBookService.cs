using BibliotecaApi.Dtos;
namespace BibliotecaApi.Services;

public interface IBookService
{
    Task<List<BookResponseDto>> GetAllAsync();
    Task<BookResponseDto?> GetByUuidAsync(string uuid);
    Task<BookResponseDto?> GetByIsbnAsync(string isbn);
    Task<BookResponseDto?> CreateAsync(BookRequestDto request);
    Task<(UpdateResult Result, BookResponseDto? Book)> UpdateAsync(string uuid, BookRequestDto request);
    Task<bool> DeleteAsync(string uuid);
}

public enum UpdateResult
{
    Success,
    NotFound,
    IsbnConflict
}

