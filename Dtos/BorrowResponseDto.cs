namespace BibliotecaApi.Dtos;

public record BorrowResponseDto
(
    string Uuid,
    string UserUuid,
    string Username,
    string BookUuid,
    string BookTitle,
    string BookIsbn,
    DateOnly BorrowDate,
    DateOnly? ReturnDate,
    string Status
);