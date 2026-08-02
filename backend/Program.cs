using Blog.Api.Data;
using Blog.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Database (Azure SQL Server via EF Core) ---
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Resolves the built SPA's hashed asset filenames for the Razor shell view.
builder.Services.AddSingleton<ViteManifest>();

// In-memory cache for individual post responses (see PostsController).
builder.Services.AddMemoryCache();

// MVC: API controllers + Razor views that host the front-end artifact.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Apply migrations on startup so the database schema is ready.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Serve the built SPA assets (JS/CSS/favicon) from wwwroot.
app.UseStaticFiles();

app.UseRouting();

// JSON API (e.g. /api/posts).
app.MapControllers();

// Default MVC route renders the SPA shell.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Any unmatched, non-API path also returns the SPA shell so client-side
// routing works on a full page load / refresh.
app.MapFallbackToController("Index", "Home");

app.Run();
