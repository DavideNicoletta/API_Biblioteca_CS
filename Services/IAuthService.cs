using BibliotecaApi.Dtos;
namespace BibliotecaApi.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(LoginRequestDto request);
}