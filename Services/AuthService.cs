using BibliotecaApi.Data;
using BibliotecaApi.Dtos;
using Microsoft.EntityFrameworkCore;


namespace BibliotecaApi.Services;

public class AuthService : IAuthService
{
    private readonly BibliotecaDbContext _db;
    private readonly IJwtService _jwtService;

    public AuthService(BibliotecaDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<string?> LoginAsync(LoginRequestDto request)
    {
        var user = await _db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if(user == null || !user.Enabled)
        {
            return null;
        }

        bool passwordOk = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if(!passwordOk)
        {
            return null;
        }

        return _jwtService.GenerateToken(user);
    }
}