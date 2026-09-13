using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

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
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
