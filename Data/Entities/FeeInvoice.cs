namespace SchoolLMS.Data.Entities;

// One billing period's fee challan for a student — backs
// Pages/AccountBook.cshtml (current fee + payment history + challan modal).
// Any component fee left at 0 simply doesn't appear in the challan, matching
// the existing UI logic.
public class FeeInvoice
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public string BillingPeriod { get; set; } = "";

    public string ChallanNumber { get; set; } = "";

    public DateOnly IssueDate { get; set; }

    public DateOnly DueDate { get; set; }

    public decimal AdmissionFee { get; set; }
    public decimal TuitionFee { get; set; }
    public decimal LibraryFee { get; set; }
    public decimal TransportFee { get; set; }
    public decimal StationeryFee { get; set; }
    public decimal UniformFee { get; set; }
    public decimal Fine { get; set; }
    public decimal OtherCharges { get; set; }
    public decimal Discount { get; set; }

    public string Status { get; set; } = "Unpaid";

    public DateOnly? PaidDate { get; set; }

    public string? ReceiptNumber { get; set; }

    public decimal TotalPayable =>
        AdmissionFee + TuitionFee + LibraryFee + TransportFee
        + StationeryFee + UniformFee + Fine + OtherCharges - Discount;
}
