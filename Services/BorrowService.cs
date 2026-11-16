using BibliotecaApi.Data;
using BibliotecaApi.Dtos;
using BibliotecaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Services;

public class BorrowService : IBorrowService
{
    private readonly BibliotecaDbContext _db;

    public BorrowService(BibliotecaDbContext db)
    {
        _db = db;
    }

    public async Task<List<BorrowResponseDto>> GetAllAsync(string userUuid, bool isAdmin)
    {
        var query = _db.Borrows.Include(b => b.User).Include(b => b.Book).AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(b => b.User.Uuid == userUuid);
        }

        var borrows = await query.ToListAsync();
        return borrows.Select(ToDto).ToList();
    }

    public async Task<BorrowResponseDto?> GetByUuidAsync(string uuid, string userUuid, bool isAdmin)
    {
        var borrow = await _db.Borrows
            .Include(b => b.User)
            .Include(b => b.Book)
            .FirstOrDefaultAsync(b => b.Uuid == uuid);

        if (borrow == null || (!isAdmin && borrow.User.Uuid != userUuid))
        {
            return null;
        }

        return ToDto(borrow);
    }

    public async Task<(BorrowResult Result, BorrowResponseDto? Borrow)> CreateAsync(string userUuid, BorrowRequestDto request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Uuid == userUuid && u.Enabled);
        var book = await _db.Books.FirstOrDefaultAsync(b => b.Uuid == request.BookUuid);

        if (user == null || book == null)
        {
            return (BorrowResult.NotFound, null);
        }

        if (book.AvailableCopies <= 0)
        {
            return (BorrowResult.NoCopiesAvailable, null);
        }

        book.AvailableCopies -= 1;

        var borrow = new Borrow
        {
            User = user,
            Book = book,
            BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = BorrowStatus.Active
        };

        _db.Borrows.Add(borrow);
        await _db.SaveChangesAsync();

        return (BorrowResult.Success, ToDto(borrow));
    }

    public async Task<(BorrowResult Result, BorrowResponseDto? Borrow)> ReturnAsync(string uuid, string userUuid, bool isAdmin)
    {
        var borrow = await _db.Borrows
            .Include(b => b.User)
            .Include(b => b.Book)
            .FirstOrDefaultAsync(b => b.Uuid == uuid);

        if (borrow == null || (!isAdmin && borrow.User.Uuid != userUuid))
        {
            return (BorrowResult.NotFound, null);
        }

        if (borrow.Status == BorrowStatus.Returned)
        {
            return (BorrowResult.AlreadyReturned, null);
        }

        borrow.Status = BorrowStatus.Returned;
        borrow.ReturnDate = DateOnly.FromDateTime(DateTime.UtcNow);
        borrow.Book.AvailableCopies += 1;

        await _db.SaveChangesAsync();

        return (BorrowResult.Success, ToDto(borrow));
    }

    public async Task<bool> DeleteAsync(string uuid)
    {
        var borrow = await _db.Borrows.FirstOrDefaultAsync(b => b.Uuid == uuid);
        if (borrow == null)
        {
            return false;
        }

        _db.Borrows.Remove(borrow);
        await _db.SaveChangesAsync();
        return true;
    }

    private static BorrowResponseDto ToDto(Borrow borrow)
    {
        return new BorrowResponseDto(
            borrow.Uuid,
            borrow.User.Uuid,
            borrow.User.Username,
            borrow.Book.Uuid,
            borrow.Book.Title,
            borrow.Book.Isbn,
            borrow.BorrowDate,
            borrow.ReturnDate,
            borrow.Status.ToString());
    }
}