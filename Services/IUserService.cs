using BibliotecaApi.Dtos;
namespace BibliotecaApi.Services;


public interface IUserService
{ 
    Task<UserResponseDto?> CreateAsync(UserRequestDto request);
    Task<List<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> GetByUuidAsync(string uuid);
}