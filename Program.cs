using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//
// Every page requires a signed-in user by default (see AuthorizeFolder
// below) except Login and ForgotPassword - real, cookie-based login, not
// the "whichever student is first in the database" placeholder every page
// used before this. See Pages/Login.cshtml.cs for how the cookie gets set,
// and Services/CurrentUserExtensions.cs for how pages read who's logged in.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/ForgotPassword");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();

// One connection string per deployment (see appsettings.json /
// appsettings.Development.json "DefaultConnection") — each school runs its
// own database against this same schema. Nothing in this app itself is
// aware of which school it's serving; that's entirely determined by which
// database the connection string points at.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Applies any pending migrations and seeds demo data on first run.
// A real school's deployment would still run migrations this way, but
// without the demo seeding — an admin screen (not yet built) would be how
// a real school enters its own students/classes/teachers/etc.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserAccount>>();
    db.Database.Migrate();
    DbSeeder.Seed(db, passwordHasher);
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
