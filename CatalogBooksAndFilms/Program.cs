using CatalogBooksAndFilms.Data;
using CatalogBooksAndFilms.Entities;
using CatalogBooksAndFilms.Services.Implementations;
using CatalogBooksAndFilms.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
// IDENTITY (Login / Register) + ROLES
// ===============================
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ===============================
// MVC (Web Interface)
// ===============================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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
// SEED: Roles + Admin + Authors + Genres
// ===============================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // 1) Roles
    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // 2) Admin user
    var adminEmail = "admin@catalog.com";
    var adminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // 3) Seed Authors
    if (!db.Authors.Any())
    {
        db.Authors.AddRange(
            new Author { Name = "J.K. Rowling" },
            new Author { Name = "George Orwell" },
            new Author { Name = "Stephen King" }
        );

        await db.SaveChangesAsync();
    }

    // 4) Seed Genres
    if (!db.Genres.Any())
    {
        db.Genres.AddRange(
            new Genre { Name = "Fantasy" },
            new Genre { Name = "Drama" },
            new Genre { Name = "Sci-Fi" },
            new Genre { Name = "Thriller" }
        );

        await db.SaveChangesAsync();
    }
}

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

app.UseAuthentication();
app.UseAuthorization();

// ===============================
// ROUTING
// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
