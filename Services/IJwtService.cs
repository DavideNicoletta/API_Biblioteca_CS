using BibliotecaApi.Models;

namespace BibliotecaApi.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}