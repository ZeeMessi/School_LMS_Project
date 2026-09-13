using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;

namespace SchoolLMS.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly AppDbContext _db;

    public DashboardModel(AppDbContext db)
    {
        _db = db;
    }

    public string SchoolName { get; set; } = "";

    public int StudentCount { get; set; }

    public int TeacherCount { get; set; }

    public int ClassCount { get; set; }

    public async Task OnGetAsync()
    {
        SchoolName = (await _db.Schools.FirstAsync()).Name;
        StudentCount = await _db.Students.CountAsync();
        TeacherCount = await _db.Teachers.CountAsync();
        ClassCount = await _db.ClassRooms.CountAsync();
    }
}
