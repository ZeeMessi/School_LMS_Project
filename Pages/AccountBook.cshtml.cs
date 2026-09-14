using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Services;

namespace SchoolLMS.Pages;

// See Overview.cshtml.cs - reads User.GetStudentId()!.Value, so must stay
// restricted to the Student role.
[Authorize(Roles = "Student")]
public class AccountBookModel : PageModel
{
    private readonly AppDbContext _db;

    public AccountBookModel(AppDbContext db)
    {
        _db = db;
    }

    // =========================================================
    // STUDENT / SCHOOL INFORMATION
    // =========================================================

    public string StudentName { get; set; } = "";

    public string ClassName { get; set; } = "";

    public string Section { get; set; } = "";

    public string SchoolName { get; set; } = "";

    public bool HasSchoolLogo { get; set; }


    // =========================================================
    // CURRENT BILLING INFORMATION
    // =========================================================

    public string BillingPeriod { get; set; } = "";

    public decimal CurrentPayable { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal Outstanding { get; set; }

    public string PaymentStatus { get; set; } = "";


    // =========================================================
    // CHALLAN INFORMATION
    // =========================================================

    public string ChallanNumber { get; set; } = "";

    public string IssueDate { get; set; } = "";

    public string DueDate { get; set; } = "";


    // =========================================================
    // FEE COMPONENTS
    // =========================================================

    public decimal AdmissionFee { get; set; }
    public decimal TuitionFee { get; set; }
    public decimal LibraryFee { get; set; }
    public decimal TransportFee { get; set; }
    public decimal StationeryFee { get; set; }
    public decimal UniformFee { get; set; }
    public decimal Fine { get; set; }
    public decimal OtherCharges { get; set; }
    public decimal Discount { get; set; }


    // =========================================================
    // PAYMENT HISTORY
    // =========================================================

    public List<PaymentHistoryItem> PaymentHistory { get; set; } = new();


    public async Task OnGetAsync()
    {
        var studentId = User.GetStudentId()!.Value;

        var student = await _db.Students
            .Include(s => s.ClassRoom)
            .FirstAsync(s => s.Id == studentId);

        StudentName = student.FullName;
        ClassName = student.ClassRoom.ClassName;
        Section = student.ClassRoom.SectionName;

        var school = await _db.Schools.FirstAsync();
        SchoolName = school.Name;
        HasSchoolLogo = school.LogoData != null;

        var invoices = await _db.FeeInvoices
            .Where(f => f.StudentId == student.Id)
            .OrderByDescending(f => f.IssueDate)
            .ToListAsync();

        // "Current" is simply the most recently-issued invoice - the rest
        // form the payment history table below it.
        var current = invoices.FirstOrDefault();

        if (current is not null)
        {
            BillingPeriod = current.BillingPeriod;
            ChallanNumber = current.ChallanNumber;
            IssueDate = current.IssueDate.ToString("dd MMMM yyyy");
            DueDate = current.DueDate.ToString("dd MMMM yyyy");
            PaymentStatus = current.Status;

            AdmissionFee = current.AdmissionFee;
            TuitionFee = current.TuitionFee;
            LibraryFee = current.LibraryFee;
            TransportFee = current.TransportFee;
            StationeryFee = current.StationeryFee;
            UniformFee = current.UniformFee;
            Fine = current.Fine;
            OtherCharges = current.OtherCharges;
            Discount = current.Discount;
            CurrentPayable = current.TotalPayable;
        }

        // Computed from every invoice, not just the current one - so this
        // agrees with the "Outstanding Dues" figure on the Overview page,
        // which is computed the same way.
        TotalPaid = invoices.Where(f => f.Status == "Paid").Sum(f => f.TotalPayable);
        Outstanding = invoices.Where(f => f.Status != "Paid").Sum(f => f.TotalPayable);

        PaymentHistory = invoices
            .Where(f => f != current)
            .Select(f => new PaymentHistoryItem
            {
                Month = f.BillingPeriod,
                Amount = f.TotalPayable,
                DueDate = f.DueDate.ToString("dd MMMM yyyy"),
                PaidDate = f.PaidDate?.ToString("dd MMMM yyyy") ?? "",
                Status = f.Status,
                ReceiptNumber = f.ReceiptNumber ?? ""
            })
            .ToList();
    }
}


// =============================================================
// PAYMENT HISTORY MODEL
// =============================================================

public class PaymentHistoryItem
{
    public string Month { get; set; } = "";

    public decimal Amount { get; set; }

    public string DueDate { get; set; } = "";

    public string PaidDate { get; set; } = "";

    public string Status { get; set; } = "";

    public string ReceiptNumber { get; set; } = "";
}
