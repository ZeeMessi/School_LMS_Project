namespace SchoolLMS.Data.Entities;

// Used only to pick which placeholder illustration shows for a teacher
// with no uploaded photo (see Services/AvatarHelper.cs) - not used
// anywhere else, so a simple binary choice is deliberately all this is.
public enum Gender
{
    Male,
    Female
}
