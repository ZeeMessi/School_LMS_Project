namespace SchoolLMS.Data.Entities;

// One database per school deployment, so in practice this table only ever
// holds a single row — but modeling it as a table (rather than appsettings
// values) means school branding/contact info can be edited from an admin
// screen later without a code change or redeploy.
public class School
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    // The logo image itself, stored in the database (not on local disk) so
    // it travels with a school's database backup/restore just like every
    // other piece of that school's data - see Program.cs's /image/school
    // endpoint for how this gets served back out as an actual image.
    public byte[]? LogoData { get; set; }

    public string? LogoContentType { get; set; }

    public string AboutUs { get; set; } = "";

    public string Address { get; set; } = "";

    public string Dial { get; set; } = "";

    public string Mobile { get; set; } = "";

    public string Fax { get; set; } = "";

    public string Email { get; set; } = "";
}
