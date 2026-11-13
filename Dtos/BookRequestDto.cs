namespace BibliotecaApi.Dtos;

public record BookRequestDto(
    string Title,
    string Author,
    string Isbn,
    decimal Price,
    int? AvailableCopies
);