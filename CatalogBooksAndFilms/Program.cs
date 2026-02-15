using CatalogBooksAndFilms.Data;
using CatalogBooksAndFilms.Services.Interfaces;
using CatalogBooksAndFilms.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CatalogBooksAndFilms.Entities;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// DATABASE (Entity Framework Core)
// ===============================
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ===============================
// IDENTITY (Login / Register)
// ===============================
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ===============================
// MVC (Web Interface)
// ===============================
builder.Services.AddControllersWithViews();

// ===============================
// SERVICE LAYER (Dependency Injection)
// ===============================
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
// ===============================
// BUILD APP
// ===============================
var app = builder.Build();

// ===============================
// HTTP PIPELINE
// ===============================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Identity
app.UseAuthentication();
app.UseAuthorization();

// ===============================
// ROUTING
// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!db.Authors.Any())
    {
        db.Authors.AddRange(
            new Author { Name = "J.K. Rowling" },
            new Author { Name = "George Orwell" },
            new Author { Name = "Stephen King" }
        );

        db.SaveChanges();
    }
}


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!db.Genres.Any())
    {
        db.Genres.AddRange(
            new Genre { Name = "Fantasy" },
            new Genre { Name = "Drama" },
            new Genre { Name = "Sci-Fi" },
            new Genre { Name = "Thriller" }
        );

        db.SaveChanges();
    }
}

app.Run();