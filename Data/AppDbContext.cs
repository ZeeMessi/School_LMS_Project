using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data.Entities;

namespace SchoolLMS.Data;

// One AppDbContext, one connection string per deployment: each school runs
// its own database using this exact schema (see appsettings.json
// "DefaultConnection"). There is no SchoolId column anywhere — isolation
// between schools comes from them simply being separate databases, not from
// filtering rows within a shared one.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<School> Schools => Set<School>();
    public DbSet<ClassRoom> ClassRooms => Set<ClassRoom>();
    public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<ExamType> ExamTypes => Set<ExamType>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamResult> ExamResults => Set<ExamResult>();
    public DbSet<FeeInvoice> FeeInvoices => Set<FeeInvoice>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<AnnouncementRead> AnnouncementReads => Set<AnnouncementRead>();
    public DbSet<FeedbackSubmission> FeedbackSubmissions => Set<FeedbackSubmission>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<CourseMaterial> CourseMaterials => Set<CourseMaterial>();
    public DbSet<TeacherRemark> TeacherRemarks => Set<TeacherRemark>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Decimal columns need explicit precision for SQL Server, or EF
        // Core will fall back to a truncating default.
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetPrecision(10);
            property.SetScale(2);
        }

        modelBuilder.Entity<ClassSubject>()
            .HasOne(cs => cs.Teacher)
            .WithMany(t => t.TaughtSubjects)
            .HasForeignKey(cs => cs.TeacherId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<UserAccount>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<UserAccount>()
            .HasOne(u => u.Student)
            .WithMany()
            .HasForeignKey(u => u.StudentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<UserAccount>()
            .HasOne(u => u.Teacher)
            .WithMany()
            .HasForeignKey(u => u.TeacherId)
            .OnDelete(DeleteBehavior.SetNull);

        // ExamResult has a cascade path to Student AND to Exam, and both of
        // those in turn cascade from ClassRoom — two cascade paths
        // converging on the same table, which SQL Server refuses to create.
        // Breaking the Exam side to Restrict keeps "delete a student wipes
        // their results" while avoiding the cycle.
        modelBuilder.Entity<ExamResult>()
            .HasOne(er => er.Exam)
            .WithMany(e => e.Results)
            .HasForeignKey(er => er.ExamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Announcement>()
            .HasOne(a => a.TargetClassRoom)
            .WithMany()
            .HasForeignKey(a => a.TargetClassRoomId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<FeedbackSubmission>()
            .HasOne(f => f.Student)
            .WithMany()
            .HasForeignKey(f => f.StudentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Announcement>()
            .HasOne(a => a.Teacher)
            .WithMany(t => t.Announcements)
            .HasForeignKey(a => a.TeacherId)
            .OnDelete(DeleteBehavior.SetNull);

        // TeacherRemark has a cascade path to Student AND to ClassSubject,
        // and both of those in turn cascade from ClassRoom - the same
        // multi-cascade-path problem ExamResult had (see above). Breaking
        // the ClassSubject side to Restrict keeps "delete a student wipes
        // their remarks" while avoiding the cycle.
        modelBuilder.Entity<TeacherRemark>()
            .HasOne(r => r.ClassSubject)
            .WithMany()
            .HasForeignKey(r => r.ClassSubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
