using BibliotecaApi.Data;
using BibliotecaApi.Dtos;
using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Models;

namespace BibliotecaApi.Services;

public class BookService : IBookService
{
    private readonly BibliotecaDbContext _db;

    public BookService(BibliotecaDbContext db)
    {
        _db = db;
    }

    public async Task<List<BookResponseDto>> GetAllAsync()
    {
        return await _db.Books
            .Select(b => new BookResponseDto(
                b.Uuid, b.Title, b.Author, b.Isbn, b.Price, b.AvailableCopies, b.CreatedAt
            )).ToListAsync();
    }

    public async Task<BookResponseDto?> GetByUuidAsync(string uuid){
        return await _db.Books
            .Where(b => b.Uuid == uuid)
            .Select(b => new BookResponseDto(b.Uuid, b.Title, b.Author, b.Isbn, b.Price, b.AvailableCopies, b.CreatedAt))
            .FirstOrDefaultAsync();
    }

    public async Task<BookResponseDto?> GetByIsbnAsync(string isbn){
        return await _db.Books
            .Where(b => b.Isbn == isbn)
            .Select(b => new BookResponseDto(b.Uuid, b.Title, b.Author, b.Isbn, b.Price, b.AvailableCopies, b.CreatedAt))
            .FirstOrDefaultAsync();
    }

    public async Task<BookResponseDto?> CreateAsync(BookRequestDto request){
        bool exist = await _db.Books.AnyAsync(b => b.Isbn == request.Isbn);
        if(exist)
        {
            return null;
        }

        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Isbn = request.Isbn,
            Price = request.Price,
            AvailableCopies = request.AvailableCopies ?? 1
        };

        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        return new BookResponseDto(book.Uuid, book.Title, book.Author, book.Isbn, book.Price, book.AvailableCopies, book.CreatedAt);
    }


    public async Task<(UpdateResult Result, BookResponseDto? Book)> UpdateAsync(string uuid, BookRequestDto request)
    {
        var book = await _db.Books.FirstOrDefaultAsync(b => b.Uuid == uuid);
        if(book == null)
        {
            return (UpdateResult.NotFound, null);
        }

        bool isbnTaken = await _db.Books.AnyAsync(b => b.Isbn == request.Isbn && b.Id != book.Id);

        if(isbnTaken)
        {
            return (UpdateResult.IsbnConflict, null);
        }

        book.Title = request.Title;
        book.Author = request.Author;
        book.Isbn = request.Isbn;
        book.Price = request.Price;
        book.AvailableCopies = request.AvailableCopies ?? book.AvailableCopies;

        await _db.SaveChangesAsync();
        return (UpdateResult.Success, new BookResponseDto(book.Uuid, book.Title, book.Author, book.Isbn, book.Price, book.AvailableCopies, book.CreatedAt));
    }

    public async Task<bool> DeleteAsync(string uuid)
    {
        var book = await _db.Books.FirstOrDefaultAsync(b => b.Uuid == uuid);
        if(book == null)
        {
            return false;
        }

        _db.Books.Remove(book);
        await _db.SaveChangesAsync();
        return true;
    }

}