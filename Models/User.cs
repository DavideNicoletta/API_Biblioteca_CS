namespace BibliotecaApi.Models;

public class User
{
    public int Id {get; set;}
    public string Uuid  {get; set;} = Guid.NewGuid().ToString();
    public string Username  {get; set;} = string.Empty;
    public string Email  {get; set;} = string.Empty;
    public string PasswordHash  {get; set;} = string.Empty;
    public bool Enabled {get; set;} = true;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public List<Role> Roles {get; set;} = new();
    public List<Borrow> Borrows {get; set;} = new();
}