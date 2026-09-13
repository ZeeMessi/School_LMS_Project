using Microsoft.AspNetCore.Mvc.RazorPages;

public class AccountBookModel : PageModel
{
    // =========================================================
    // STUDENT INFORMATION
    //
    // FUTURE:
    // These values will come from the database.
    // =========================================================

    public string StudentName { get; set; } = "Ali Ahmed";

    public string ClassName { get; set; } = "Class 5";

    public string Section { get; set; } = "Section A";


    // =========================================================
    // CURRENT BILLING INFORMATION
    // =========================================================

    public string BillingPeriod { get; set; } = "August 2026";

    public decimal CurrentPayable { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal Outstanding { get; set; }

    public string PaymentStatus { get; set; } = "Unpaid";


    // =========================================================
    // CHALLAN INFORMATION
    //
    // These details are intentionally displayed only inside
    // the View/Download Challan area.
    // =========================================================

    public string ChallanNumber { get; set; } = "SCH-2026-000125";

    public string IssueDate { get; set; } = "01 August 2026";

    public string DueDate { get; set; } = "15 August 2026";


    // =========================================================
    // FEE COMPONENTS
    //
    // Any value of 0 means that fee will not appear in the
    // challan.
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

    public List<PaymentHistoryItem> PaymentHistory { get; set; }
        = new List<PaymentHistoryItem>();


    // =========================================================
    // PAGE LOAD
    //
    // TEMPORARY SAMPLE DATA ONLY.
    //
    // LATER:
    // Replace this section with database/service calls.
    // =========================================================

    public void OnGet()
    {
        TuitionFee = 10000;

        AdmissionFee = 0;

        LibraryFee = 0;

        TransportFee = 2000;

        StationeryFee = 0;

        UniformFee = 0;

        Fine = 0;

        OtherCharges = 0;

        Discount = 0;


        CurrentPayable =
            AdmissionFee
            + TuitionFee
            + LibraryFee
            + TransportFee
            + StationeryFee
            + UniformFee
            + Fine
            + OtherCharges
            - Discount;


        TotalPaid = 20000;

        Outstanding = CurrentPayable;


        PaymentStatus = "Unpaid";


        PaymentHistory = new List<PaymentHistoryItem>
        {
            new PaymentHistoryItem
            {
                Month = "July 2026",
                Amount = 10000,
                DueDate = "15 July 2026",
                PaidDate = "13 July 2026",
                Status = "Paid",
                ReceiptNumber = "REC-2026-00091"
            },

            new PaymentHistoryItem
            {
                Month = "June 2026",
                Amount = 10000,
                DueDate = "15 June 2026",
                PaidDate = "14 June 2026",
                Status = "Paid",
                ReceiptNumber = "REC-2026-00072"
            },

            new PaymentHistoryItem
            {
                Month = "May 2026",
                Amount = 10000,
                DueDate = "15 May 2026",
                PaidDate = "—",
                Status = "Unpaid",
                ReceiptNumber = ""
            }
        };
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