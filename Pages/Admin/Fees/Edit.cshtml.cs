using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;

namespace SchoolLMS.Pages.Admin.Fees;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public int? Id { get; set; }

    [BindProperty]
    public int StudentId { get; set; }

    [BindProperty]
    public string BillingPeriod { get; set; } = "";

    [BindProperty]
    public string ChallanNumber { get; set; } = "";

    [BindProperty]
    public DateOnly IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [BindProperty]
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(14));

    [BindProperty]
    public decimal AdmissionFee { get; set; }

    [BindProperty]
    public decimal TuitionFee { get; set; }

    [BindProperty]
    public decimal LibraryFee { get; set; }

    [BindProperty]
    public decimal TransportFee { get; set; }

    [BindProperty]
    public decimal StationeryFee { get; set; }

    [BindProperty]
    public decimal UniformFee { get; set; }

    [BindProperty]
    public decimal Fine { get; set; }

    [BindProperty]
    public decimal OtherCharges { get; set; }

    [BindProperty]
    public decimal Discount { get; set; }

    [BindProperty]
    public string Status { get; set; } = "Unpaid";

    [BindProperty]
    public DateOnly? PaidDate { get; set; }

    [BindProperty]
    public string ReceiptNumber { get; set; } = "";

    public bool IsNew => Id is null or 0;

    public List<Student> AllStudents { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        AllStudents = await _db.Students.Include(s => s.ClassRoom).OrderBy(s => s.RollNumber).ToListAsync();

        if (!IsNew)
        {
            var invoice = await _db.FeeInvoices.FirstAsync(f => f.Id == Id);
            StudentId = invoice.StudentId;
            BillingPeriod = invoice.BillingPeriod;
            ChallanNumber = invoice.ChallanNumber;
            IssueDate = invoice.IssueDate;
            DueDate = invoice.DueDate;
            AdmissionFee = invoice.AdmissionFee;
            TuitionFee = invoice.TuitionFee;
            LibraryFee = invoice.LibraryFee;
            TransportFee = invoice.TransportFee;
            StationeryFee = invoice.StationeryFee;
            UniformFee = invoice.UniformFee;
            Fine = invoice.Fine;
            OtherCharges = invoice.OtherCharges;
            Discount = invoice.Discount;
            Status = invoice.Status;
            PaidDate = invoice.PaidDate;
            ReceiptNumber = invoice.ReceiptNumber ?? "";
        }
        else if (AllStudents.Count > 0)
        {
            StudentId = AllStudents[0].Id;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        AllStudents = await _db.Students.Include(s => s.ClassRoom).OrderBy(s => s.RollNumber).ToListAsync();

        if (string.IsNullOrWhiteSpace(BillingPeriod))
        {
            ErrorMessage = "Billing period is required.";
            return Page();
        }

        FeeInvoice invoice;
        if (IsNew)
        {
            invoice = new FeeInvoice { StudentId = StudentId };
            _db.FeeInvoices.Add(invoice);
        }
        else
        {
            invoice = await _db.FeeInvoices.FirstAsync(f => f.Id == Id);
            invoice.StudentId = StudentId;
        }

        invoice.BillingPeriod = BillingPeriod;

        // An empty text input binds to null, not "", for string
        // properties (a standard-but-surprising ASP.NET Core model
        // binding behavior) - ChallanNumber's database column is
        // NOT NULL, so a blank field here would otherwise throw a
        // SqlException at SaveChangesAsync instead of just saving "".
        invoice.ChallanNumber = ChallanNumber ?? "";
        invoice.IssueDate = IssueDate;
        invoice.DueDate = DueDate;
        invoice.AdmissionFee = AdmissionFee;
        invoice.TuitionFee = TuitionFee;
        invoice.LibraryFee = LibraryFee;
        invoice.TransportFee = TransportFee;
        invoice.StationeryFee = StationeryFee;
        invoice.UniformFee = UniformFee;
        invoice.Fine = Fine;
        invoice.OtherCharges = OtherCharges;
        invoice.Discount = Discount;
        invoice.Status = Status;
        invoice.PaidDate = Status == "Paid" ? PaidDate ?? DateOnly.FromDateTime(DateTime.Today) : null;
        invoice.ReceiptNumber = Status == "Paid" ? ReceiptNumber : null;

        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
