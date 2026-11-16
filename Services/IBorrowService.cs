using BibliotecaApi.Dtos;

namespace BibliotecaApi.Services;


public interface IBorrowService
{
    Task<List<BorrowResponseDto>> GetAllAsync(string userUuid, bool isAdmin);
    Task<BorrowResponseDto?> GetByUuidAsync(string uuid, string userUuid, bool isAdmin);
    Task<(BorrowResult Result, BorrowResponseDto? Borrow)> CreateAsync(string userUuid, BorrowRequestDto request);
    Task<(BorrowResult Result, BorrowResponseDto? Borrow)> ReturnAsync(string uuid, string userUuid, bool isAdmin);
    Task<bool> DeleteAsync(string uuid);

}

public enum BorrowResult
{
    Success,
    NotFound,
    NoCopiesAvailable,
    AlreadyReturned
}       