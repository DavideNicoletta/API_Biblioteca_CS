namespace BibliotecaApi.Models;


public class Book
{
    public int Id {get; set;}
    public string Uuid  {get; set;} = Guid.NewGuid().ToString();
    public string Title  {get; set;} = string.Empty;
    public string Author  {get; set;} = string.Empty;
    public string Isbn  {get; set;} = string.Empty;
    public decimal Price  {get; set;}
    public int AvailableCopies  {get; set;} = 1;
    public DateTime CreatedAt  {get; set;} = DateTime.UtcNow;
    public List<Borrow> Borrows {get; set;} = new();
}