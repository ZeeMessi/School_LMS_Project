namespace SchoolLMS.Data.Entities;

// Assignment, Handout, and Quiz all turned out to be the exact same shape
// (a title, an optional description/due date, an optional file) - one
// entity with a type tag instead of three near-identical tables.
public enum MaterialType
{
    Assignment,
    Handout,
    Quiz
}
