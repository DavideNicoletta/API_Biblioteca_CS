namespace BibliotecaApi.Dtos;


public record UserResponseDto
(
    string Uuid,
    string Username,
    string Email,
    bool Enabled,
    DateTime CreatedAt,
    List<string> Roles
);