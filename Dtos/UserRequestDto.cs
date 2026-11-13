namespace BibliotecaApi.Dtos;



public record UserRequestDto
(
    string Username,
    string Email,
    string Password
);