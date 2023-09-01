using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BookDb>(opt => opt.UseInMemoryDatabase("BookList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", () => "Kirjasovellus!");

app.MapGet("/books", async (BookDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/books/{id}", async (int id, BookDb db) =>
    await db.Todos.FindAsync(id)
        is Book book
            ? Results.Ok(book)
            : Results.NotFound());

app.MapPost("/books", async (Book book, BookDb db) =>
{
    db.Todos.Add(book);
    await db.SaveChangesAsync();

    return Results.Created($"/books/{book.Id}", book);
});

app.MapPut("/books/{id}", async (int id, Book inputBook, BookDb db) =>
{
    var book = await db.Todos.FindAsync(id);

    if (book is null) return Results.NotFound();

    book.Name = inputBook.Name;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/books/{id}", async (int id, BookDb db) =>
{
    if (await db.Todos.FindAsync(id) is Book book)
    {
        db.Todos.Remove(book);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();

public class Book
{
    public int Id { get; set; }
    public string? Name { get; set; }

}

class BookDb : DbContext
{
    public BookDb(DbContextOptions<BookDb> options)
        : base(options) { }

    public DbSet<Book> Todos => Set<Book>();
}