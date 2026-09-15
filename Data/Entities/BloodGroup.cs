namespace SchoolLMS.Data.Entities;

// Enum members can't contain "+"/"-", so display text (A+, A-, ...) is
// produced by Services/BloodGroupHelper rather than ToString() here.
// Nullable everywhere it's used - not every school records this, and
// there's no honest "unknown" member to default an enum to instead.
public enum BloodGroup
{
    APositive,
    ANegative,
    BPositive,
    BNegative,
    ABPositive,
    ABNegative,
    OPositive,
    ONegative
}
