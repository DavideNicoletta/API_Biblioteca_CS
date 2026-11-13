namespace BibliotecaApi.Dtos;

public record BookResponseDto(
    string Uuid,
    string Title,
    string Author,
    string Isbn,
    decimal Price,
    int AvailableCopies,
    DateTime CreatedAt);