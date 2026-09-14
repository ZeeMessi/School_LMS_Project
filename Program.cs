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

// Photos/logo are stored as bytes in the database (see School.LogoData,
// Student.PhotoData, Teacher.PhotoData) rather than as files on local
// disk, so they travel with a school's database backup/restore like every
// other piece of that school's data. These three endpoints are the only
// way that data ever becomes an actual <img>-loadable URL.
//
// The school logo needs to render on Login itself (an anonymous page), so
// it's explicitly AllowAnonymous; student/teacher photos are only ever
// shown on pages that already require a signed-in user, so those require
// one too - not because a photo is especially sensitive, but so a photo
// can't be enumerated by guessing ids without even logging in.
app.MapGet("/image/school", async (AppDbContext db) =>
{
    var school = await db.Schools.FirstOrDefaultAsync();
    return school?.LogoData is { } bytes
        ? Results.File(bytes, school.LogoContentType ?? "application/octet-stream")
        : Results.NotFound();
}).AllowAnonymous();

app.MapGet("/image/student/{id:int}", async (int id, AppDbContext db) =>
{
    var student = await db.Students.FindAsync(id);
    return student?.PhotoData is { } bytes
        ? Results.File(bytes, student.PhotoContentType ?? "application/octet-stream")
        : Results.NotFound();
}).RequireAuthorization();

app.MapGet("/image/teacher/{id:int}", async (int id, AppDbContext db) =>
{
    var teacher = await db.Teachers.FindAsync(id);
    return teacher?.PhotoData is { } bytes
        ? Results.File(bytes, teacher.PhotoContentType ?? "application/octet-stream")
        : Results.NotFound();
}).RequireAuthorization();

app.Run();
