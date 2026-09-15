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

// Applies any pending migrations on every startup, in every environment -
// a real school's deployment needs this exactly as much as local dev does.
//
// What gets seeded into an empty database differs by environment: local
// Development gets the full demo school (fake students/teachers/grades/
// etc, so every page has something to show); anywhere else gets just a
// blank School row and one Admin login with a randomly-generated
// password printed to the startup log - see DbSeeder.SeedProductionDefaults.
// A real school then enters its own students/teachers/classes/etc through
// the Admin screens, not this seeder.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserAccount>>();
    db.Database.Migrate();

    if (app.Environment.IsDevelopment())
    {
        DbSeeder.SeedDemoData(db, passwordHasher);
    }
    else
    {
        DbSeeder.SeedProductionDefaults(db, passwordHasher);
    }
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

// Same "stored as bytes in the database" pattern as the /image endpoints
// above, for assignment/handout/quiz attachments and announcement
// attachments - fileDownloadName sets a real Content-Disposition so the
// browser downloads/saves with the original filename instead of a bare
// id. Any signed-in user can fetch any attachment by id (no per-resource
// ownership check) - same simplification already made for photos, since
// everyone authenticated here belongs to the same one school.
app.MapGet("/file/material/{id:int}", async (int id, AppDbContext db) =>
{
    var material = await db.CourseMaterials.FindAsync(id);
    return material?.FileData is { } bytes
        ? Results.File(bytes, material.FileContentType ?? "application/octet-stream", material.FileName)
        : Results.NotFound();
}).RequireAuthorization();

app.MapGet("/file/announcement/{id:int}", async (int id, AppDbContext db) =>
{
    var announcement = await db.Announcements.FindAsync(id);
    return announcement?.AttachmentData is { } bytes
        ? Results.File(bytes, announcement.AttachmentContentType ?? "application/octet-stream", announcement.AttachmentFileName)
        : Results.NotFound();
}).RequireAuthorization();

app.Run();
