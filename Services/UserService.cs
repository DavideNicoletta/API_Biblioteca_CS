using BibliotecaApi.Data;
using BibliotecaApi.Dtos;
using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Models;

namespace BibliotecaApi.Services;


public class UserService : IUserService
{

    private readonly BibliotecaDbContext _db;

    public UserService(BibliotecaDbContext db)
    {
        _db = db;
    }

    public async Task<UserResponseDto?> CreateAsync(UserRequestDto request)
    {
        bool exists = await _db.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email);
        if (exists)
        {
            return null;
        }

        var roleUser = await _db.Roles.FirstAsync(r => r.Name == "ROLE_USER");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Roles = { roleUser }
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return ToDto(user);
    }

    public async Task<List<UserResponseDto>> GetAllAsync()
    {
        var users = await _db.Users.Include(u => u.Roles).ToListAsync();
        return users.Select(ToDto).ToList();
    }

    public async Task<UserResponseDto?> GetByUuidAsync(string uuid)
    {
        var user = await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Uuid == uuid);
        return user == null ? null : ToDto(user);
    }

    private static UserResponseDto ToDto(User user)
    {
        return new UserResponseDto(
            user.Uuid,
            user.Username,
            user.Email,
            user.Enabled,
            user.CreatedAt,
            user.Roles.Select(r => r.Name).ToList());
    }
}