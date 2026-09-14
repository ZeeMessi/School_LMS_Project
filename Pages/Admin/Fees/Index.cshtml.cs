using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;

namespace SchoolLMS.Pages.Admin.Fees;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<InvoiceRow> Invoices { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Invoices = await _db.FeeInvoices
            .Include(f => f.Student)
            .OrderByDescending(f => f.IssueDate)
            .Select(f => new InvoiceRow(
                f.Id,
                f.Student.FullName,
                f.BillingPeriod,
                f.TotalPayable,
                f.Status,
                f.DueDate))
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var invoice = await _db.FeeInvoices.FindAsync(id);
        if (invoice is not null)
        {
            _db.FeeInvoices.Remove(invoice);
            await _db.SaveChangesAsync();
            StatusMessage = "Invoice removed.";
        }

        return RedirectToPage();
    }
}

public record InvoiceRow(int Id, string StudentName, string BillingPeriod, decimal TotalPayable, string Status, DateOnly DueDate);
