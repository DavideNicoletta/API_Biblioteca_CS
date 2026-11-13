using BibliotecaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Data;

public class BibliotecaDbContext : DbContext
{
    public BibliotecaDbContext(DbContextOptions<BibliotecaDbContext> options) : base(options) 
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Borrow> Borrows => Set<Borrow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasIndex(u => u.Username).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Uuid).IsUnique();
            e.HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity(j => j.ToTable("user_roles"));

            e.Property(u => u.Uuid).HasMaxLength(36).IsRequired();
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
            e.Property(u => u.Email).HasMaxLength(100).IsRequired();
            e.Property(u => u.PasswordHash).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Book>(e =>
        {
            e.ToTable("books");
            e.HasIndex(b => b.Isbn).IsUnique();
            e.HasIndex(b => b.Uuid).IsUnique();

            e.Property(b => b.Price).HasPrecision(10, 2);
            e.Property(b => b.Uuid).HasMaxLength(36).IsRequired();
            e.Property(b => b.Title).HasMaxLength(200).IsRequired();
            e.Property(b => b.Author).HasMaxLength(150).IsRequired();
            e.Property(b => b.Isbn).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<Role>(e =>
        {
            e.ToTable("roles");
            e.HasIndex(r => r.Name).IsUnique();
            e.HasData(
                new Role { Id = 1, Name = "ROLE_USER" },
                new Role { Id = 2, Name = "ROLE_ADMIN" });

            e.Property(r => r.Name).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Borrow>(e =>
        {
            e.ToTable("borrows");
            e.HasIndex(b => b.Uuid).IsUnique();

            e.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(b => b.Uuid).HasMaxLength(36).IsRequired();
        });
    }

}