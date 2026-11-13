namespace BibliotecaApi.Models;


public enum BorrowStatus
{
    Active,
    Returned
}


public class Borrow
{

    public int Id {get; set;}
    public string Uuid {get; set;} = Guid.NewGuid().ToString();
    public int UserId {get; set;}
    public User User {get; set;} = null!;
    public int BookId {get;set;}
    public Book Book {get; set;} = null!;
    public DateOnly BorrowDate {get; set;}
    public DateOnly? ReturnDate {get; set;}
    public BorrowStatus Status {get; set;} = BorrowStatus.Active;
}