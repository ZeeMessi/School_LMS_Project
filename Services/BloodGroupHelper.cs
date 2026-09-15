using SchoolLMS.Data.Entities;

namespace SchoolLMS.Services;

// BloodGroup enum members can't be named "A+"/"A-"/etc, so this is the
// one place that maps between the two - used by every Admin form with a
// blood group dropdown, and anywhere one is displayed.
public static class BloodGroupHelper
{
    public static string Label(BloodGroup value) => value switch
    {
        BloodGroup.APositive => "A+",
        BloodGroup.ANegative => "A-",
        BloodGroup.BPositive => "B+",
        BloodGroup.BNegative => "B-",
        BloodGroup.ABPositive => "AB+",
        BloodGroup.ABNegative => "AB-",
        BloodGroup.OPositive => "O+",
        BloodGroup.ONegative => "O-",
        _ => value.ToString()
    };

    public static IReadOnlyList<(BloodGroup Value, string Label)> Options { get; } =
        Enum.GetValues<BloodGroup>().Select(v => (v, Label(v))).ToList();
}
