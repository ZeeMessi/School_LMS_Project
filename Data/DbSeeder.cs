using Microsoft.AspNetCore.Identity;
using SchoolLMS.Data.Entities;

namespace SchoolLMS.Data;

// Seeds the database with the same demo content that used to be hardcoded
// across the .cshtml.cs files, so converting a page to read from the
// database doesn't change what's on screen. Runs once, only when the
// database is empty (see Program.cs) — a real deployment for an actual
// school replaces this with its own data via an admin UI, not this seeder.
//
// NOTE: the original hardcoded pages actually disagreed with each other —
// the header showed "Ali Raza", but Grade.cshtml showed "Ahmed Khan" and
// AccountBook.cshtml showed "Ali Ahmed" as if they were three different
// demo students. Since there's now one real Student row, "Ali Raza" (the
// name shown in the persistent header) is the one used everywhere.
public static class DbSeeder
{
    // Used in Development (see Program.cs) - fills in the full demo
    // school (classes, teachers, a student, results, fees, announcements)
    // so every page has real-looking data to show right away.
    public static void SeedDemoData(AppDbContext db, IPasswordHasher<UserAccount> passwordHasher)
    {
        if (db.Schools.Any())
        {
            return; // already seeded
        }

        var school = new School
        {
            Name = "Sample School",
            AboutUs = "School information will appear here.",
            Address = "School Address",
            Dial = "+92 (51-111-8-88-80)",
            Mobile = "0304-1234567",
            Fax = "+92 (51) 1234567",
            Email = "school@example.com"
        };
        db.Schools.Add(school);

        var classRoom = new ClassRoom
        {
            ClassName = "Class 5",
            SectionName = "Section A",
            AcademicYear = "2026 - 2027"
        };
        db.ClassRooms.Add(classRoom);

        var teachers = new[]
        {
            new Teacher { FullName = "Mrs. Khan", Gender = Gender.Female },
            new Teacher { FullName = "Mr. Ahmed", Gender = Gender.Male },
            new Teacher { FullName = "Ms. Fatima", Gender = Gender.Female },
            new Teacher { FullName = "Mr. Bilal", Gender = Gender.Male },
            new Teacher { FullName = "Mrs. Sana", Gender = Gender.Female },
            new Teacher { FullName = "Mr. Imran", Gender = Gender.Male },
            new Teacher { FullName = "Mrs. Noreen", Gender = Gender.Female },
            new Teacher { FullName = "Mr. Yousuf", Gender = Gender.Male },
        };
        db.Teachers.AddRange(teachers);

        var subjectsByTeacher = new (string Subject, Teacher Teacher)[]
        {
            ("English", teachers[0]),
            ("Mathematics", teachers[1]),
            ("Science", teachers[2]),
            ("Pakistan Studies", teachers[3]),
            ("Drawing", teachers[4]),
            ("Art", teachers[5]),
            ("Social Studies", teachers[6]),
            ("Urdu", teachers[7]),
        };
        var classSubjects = subjectsByTeacher
            .Select(x => new ClassSubject { ClassRoom = classRoom, Name = x.Subject, Teacher = x.Teacher })
            .ToList();
        db.ClassSubjects.AddRange(classSubjects);

        var student = new Student
        {
            FullName = "Ali Raza",
            RollNumber = "10-A-042",
            ClassRoom = classRoom,
            Gender = Gender.Male,
            GuardianName = "Muhammad Raza",
            GuardianContactNumber = "0301-1234567",
            Address = "House 12, Street 5, Gulshan-e-Iqbal, Karachi",
            BloodGroup = BloodGroup.OPositive,
            CnicOrBFormNumber = "42101-1234567-1"
        };
        db.Students.Add(student);

        SeedAttendance(db, student);

        var (examTypes, quarterly) = SeedExamTypes(db);
        var monthlyTest = SeedMonthlyTestResults(db, classRoom, student);
        var quarterlyExams = SeedUpcomingExams(db, classRoom, quarterly);
        SeedQuarterlyResults(db, quarterlyExams, student);

        SeedFeeInvoices(db, student);
        SeedAnnouncements(db, classRoom);

        var mathematics = classSubjects.First(cs => cs.Name == "Mathematics");
        SeedCourseContent(db, mathematics, student);

        // teachers[1] is "Mr. Ahmed", the Mathematics teacher - chosen as
        // the demo teacher account since Mathematics already has seeded
        // exam results (see SeedMonthlyTestResults), so the teacher
        // dashboard has real data to show right away.
        SeedUserAccounts(db, passwordHasher, student, teachers[1]);

        db.SaveChanges();
    }

    // Used everywhere else (see Program.cs) - a real school's database
    // should start with nothing pretending to be real data in it. Creates
    // just enough to log in and take it from there: a School row (blank,
    // filled in from Admin > School) and one Admin account with a
    // randomly-generated first-run password, printed once to the
    // console/log for whoever is running the deployment to retrieve -
    // never hardcoded, since a fixed default posted in a public repo or
    // chat history would be a known password for every school running
    // this unmodified. Pages/ChangePassword.cshtml is how that Admin
    // account (or anyone's) changes its own password afterward.
    public static void SeedProductionDefaults(AppDbContext db, IPasswordHasher<UserAccount> passwordHasher)
    {
        if (db.Schools.Any())
        {
            return; // already set up
        }

        db.Schools.Add(new School
        {
            Name = "Your School Name",
            AboutUs = "",
            Address = "",
            Dial = "",
            Mobile = "",
            Fax = "",
            Email = ""
        });

        var adminAccount = new UserAccount
        {
            Username = "admin",
            Role = UserRole.Admin
        };

        var temporaryPassword = GenerateTemporaryPassword();
        adminAccount.PasswordHash = passwordHasher.HashPassword(adminAccount, temporaryPassword);
        db.UserAccounts.Add(adminAccount);

        db.SaveChanges();

        Console.WriteLine("=================================================================");
        Console.WriteLine(" First run: created the initial Admin login.");
        Console.WriteLine("   Username: admin");
        Console.WriteLine($"   Password: {temporaryPassword}");
        Console.WriteLine(" Log in, then change this password immediately (top-right menu)");
        Console.WriteLine(" and fill in the School page under Admin. This message will not");
        Console.WriteLine(" appear again - the password is not stored anywhere in plain text,");
        Console.WriteLine(" including in these logs after this run.");
        Console.WriteLine("=================================================================");
    }

    private static string GenerateTemporaryPassword()
    {
        // Excludes visually-ambiguous characters (0/O, 1/l/I) since this
        // is meant to be read off a console and retyped once.
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";

        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(14);
        var password = new char[bytes.Length];
        for (var i = 0; i < bytes.Length; i++)
        {
            password[i] = chars[bytes[i] % chars.Length];
        }

        return new string(password);
    }

    private static void SeedUserAccounts(
        AppDbContext db,
        IPasswordHasher<UserAccount> passwordHasher,
        Student student,
        Teacher teacher)
    {
        var studentAccount = new UserAccount
        {
            Username = "ali.raza",
            Role = UserRole.Student,
            Student = student
        };
        studentAccount.PasswordHash = passwordHasher.HashPassword(studentAccount, "Student@123");
        db.UserAccounts.Add(studentAccount);

        var teacherAccount = new UserAccount
        {
            Username = "mr.ahmed",
            Role = UserRole.Teacher,
            Teacher = teacher
        };
        teacherAccount.PasswordHash = passwordHasher.HashPassword(teacherAccount, "Teacher@123");
        db.UserAccounts.Add(teacherAccount);

        var adminAccount = new UserAccount
        {
            Username = "admin",
            Role = UserRole.Admin
        };
        adminAccount.PasswordHash = passwordHasher.HashPassword(adminAccount, "Admin@123");
        db.UserAccounts.Add(adminAccount);
    }

    private static void SeedAttendance(AppDbContext db, Student student)
    {
        // August 2026, matching what Pages/Attendance.cshtml used to show
        // as static markup. Weekends are Saturday/Sunday.
        var notRecorded = new HashSet<int> { 14, 28 };

        for (var day = 1; day <= 31; day++)
        {
            var date = new DateOnly(2026, 8, day);

            var status = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
                ? AttendanceStatus.Weekend
                : notRecorded.Contains(day)
                    ? AttendanceStatus.NotRecorded
                    : day is 5 or 19
                        ? AttendanceStatus.Absent
                        : AttendanceStatus.Present;

            db.AttendanceRecords.Add(new AttendanceRecord
            {
                Student = student,
                Date = date,
                Status = status
            });
        }
    }

    private static (List<ExamType> types, ExamType quarterly) SeedExamTypes(AppDbContext db)
    {
        var types = new List<ExamType>
        {
            new() { Code = "QUARTERLY", Name = "Quarterly", Icon = "📝", IsActive = true },
            new() { Code = "BIANNUAL", Name = "Bi-Annual", Icon = "📚", IsActive = false },
            new() { Code = "ANNUAL", Name = "Annual", Icon = "🏆", IsActive = false },
            new() { Code = "MONTHLY", Name = "Monthly Test", Icon = "📝", IsActive = false },
        };
        db.ExamTypes.AddRange(types);
        return (types, types[0]);
    }

    private static ExamType SeedMonthlyTestResults(AppDbContext db, ClassRoom classRoom, Student student)
    {
        var monthlyType = db.ExamTypes.Local.First(t => t.Code == "MONTHLY");

        // Matches Pages/Grade.cshtml's "Monthly Test" results table.
        var results = new (string Subject, int Total, int Obtained, string Grade, string Remarks)[]
        {
            ("English", 100, 88, "A", "Very Good"),
            ("Mathematics", 100, 92, "A+", "Outstanding"),
            ("Science", 100, 81, "A", "Very Good"),
            ("Urdu", 100, 76, "B+", "Good"),
            ("Drawing", 50, 45, "A+", "Excellent"),
        };

        foreach (var (subject, total, obtained, grade, remarks) in results)
        {
            var exam = new Exam
            {
                ExamType = monthlyType,
                ClassRoom = classRoom,
                Subject = subject,
                Date = new DateTime(2026, 7, 20),
                StartTime = "09:00 AM",
                EndTime = "11:00 AM",
                Status = "Completed",
                CreatedAt = new DateTime(2026, 7, 20)
            };
            db.Exams.Add(exam);

            db.ExamResults.Add(new ExamResult
            {
                Student = student,
                Exam = exam,
                TotalMarks = total,
                ObtainedMarks = obtained,
                Grade = grade,
                Remarks = remarks,
                RecordedAt = new DateTime(2026, 7, 21)
            });
        }

        return monthlyType;
    }

    private static List<Exam> SeedUpcomingExams(AppDbContext db, ClassRoom classRoom, ExamType quarterly)
    {
        // Matches ExamScheduleModel.LoadExams()'s hardcoded sample data.
        var exams = new (string Subject, string Code, DateTime Date, string Room, string Invigilator, string Icon)[]
        {
            ("English", "ENG-05", new DateTime(2026, 9, 5), "Room 204", "Mr. Ahmed Khan", "🇬🇧"),
            ("Mathematics", "MTH-05", new DateTime(2026, 9, 7), "Room 204", "Ms. Sara Ali", "📐"),
            ("Science", "SCI-05", new DateTime(2026, 9, 9), "Room 204", "Mr. Bilal Ahmed", "🔬"),
            ("Urdu", "URD-05", new DateTime(2026, 9, 11), "Room 204", "Mrs. Ayesha Khan", "📖"),
            ("Social Studies", "SST-05", new DateTime(2026, 9, 14), "Room 205", "Mr. Hamza", "🌍"),
            ("Drawing", "DRW-05", new DateTime(2026, 9, 16), "Art Room", "Ms. Hina", "🎨"),
        };

        var createdExams = new List<Exam>();

        foreach (var (subject, code, date, room, invigilator, icon) in exams)
        {
            var exam = new Exam
            {
                ExamType = quarterly,
                ClassRoom = classRoom,
                Subject = subject,
                SubjectCode = code,
                Icon = icon,
                Date = date,
                StartTime = "09:00 AM",
                EndTime = subject == "Drawing" ? "10:30 AM" : "11:00 AM",
                Room = room,
                Invigilator = invigilator,
                Status = "Upcoming",
                CreatedAt = new DateTime(2026, 8, 25)
            };
            db.Exams.Add(exam);
            createdExams.Add(exam);
        }

        return createdExams;
    }

    private static void SeedQuarterlyResults(AppDbContext db, List<Exam> quarterlyExams, Student student)
    {
        // Only the exams already sat (5, 7, 9, 11, 14 September) get a
        // recorded result - Drawing (16 September) hasn't happened yet as
        // of the seed data's "today", so it stays exam-schedule-only,
        // matching how a real school wouldn't have marks for a test nobody
        // has taken. This also gives Pages/Progress.cshtml.cs a second
        // real sitting to compare against the Monthly Test above, so
        // "improvement" is a genuine computed comparison instead of
        // fabricated trend data.
        var results = new (string Subject, int Total, int Obtained, string Grade, string Remarks)[]
        {
            ("English", 100, 91, "A+", "Excellent improvement"),
            ("Mathematics", 100, 89, "A", "Slight dip, still strong"),
            ("Science", 100, 86, "A", "Good improvement"),
            ("Urdu", 100, 82, "A", "Good improvement"),
            ("Social Studies", 100, 78, "B+", "Good"),
        };

        foreach (var (subject, total, obtained, grade, remarks) in results)
        {
            var exam = quarterlyExams.First(e => e.Subject == subject);

            db.ExamResults.Add(new ExamResult
            {
                Student = student,
                Exam = exam,
                TotalMarks = total,
                ObtainedMarks = obtained,
                Grade = grade,
                Remarks = remarks,
                RecordedAt = exam.Date.AddDays(1)
            });
        }
    }

    private static void SeedFeeInvoices(AppDbContext db, Student student)
    {
        // Matches AccountBookModel's current fee + payment history.
        db.FeeInvoices.Add(new FeeInvoice
        {
            Student = student,
            BillingPeriod = "August 2026",
            ChallanNumber = "SCH-2026-000125",
            IssueDate = new DateOnly(2026, 8, 1),
            DueDate = new DateOnly(2026, 8, 15),
            TuitionFee = 10000,
            TransportFee = 2000,
            Status = "Unpaid"
        });

        db.FeeInvoices.Add(new FeeInvoice
        {
            Student = student,
            BillingPeriod = "July 2026",
            ChallanNumber = "SCH-2026-000091",
            IssueDate = new DateOnly(2026, 7, 1),
            DueDate = new DateOnly(2026, 7, 15),
            TuitionFee = 10000,
            Status = "Paid",
            PaidDate = new DateOnly(2026, 7, 13),
            ReceiptNumber = "REC-2026-00091"
        });

        db.FeeInvoices.Add(new FeeInvoice
        {
            Student = student,
            BillingPeriod = "June 2026",
            ChallanNumber = "SCH-2026-000072",
            IssueDate = new DateOnly(2026, 6, 1),
            DueDate = new DateOnly(2026, 6, 15),
            TuitionFee = 10000,
            Status = "Paid",
            PaidDate = new DateOnly(2026, 6, 14),
            ReceiptNumber = "REC-2026-00072"
        });

        db.FeeInvoices.Add(new FeeInvoice
        {
            Student = student,
            BillingPeriod = "May 2026",
            ChallanNumber = "SCH-2026-000050",
            IssueDate = new DateOnly(2026, 5, 1),
            DueDate = new DateOnly(2026, 5, 15),
            TuitionFee = 10000,
            Status = "Unpaid"
        });
    }

    private static void SeedAnnouncements(AppDbContext db, ClassRoom classRoom)
    {
        // Matches AnnouncementModel's hardcoded sample list.
        db.Announcements.AddRange(
            new Announcement
            {
                Title = "Parent-Teacher Meeting",
                Category = "Notice",
                Date = new DateTime(2026, 8, 13),
                ShortDescription = "Parents are requested to attend the upcoming parent-teacher meeting.",
                FullDescription = "The school administration has announced a parent-teacher meeting. Parents will receive further details regarding the meeting schedule and venue.",
                IssuedBy = "School Administration",
                IsImportant = true
            },
            new Announcement
            {
                Title = "Annual Examination Schedule",
                Category = "Notice",
                Date = new DateTime(2026, 8, 10),
                ShortDescription = "The annual examination schedule has been announced.",
                FullDescription = "The annual examination schedule has been published by the school administration. Students should check the examination schedule carefully.",
                IssuedBy = "Examination Department",
                IsImportant = true
            },
            new Announcement
            {
                Title = "School Activity",
                Category = "Event",
                Date = new DateTime(2026, 8, 7),
                ShortDescription = "A school activity has been scheduled for students.",
                FullDescription = "Students are requested to participate in the scheduled school activity.",
                IssuedBy = "School Administration",
                IsImportant = false,
                TargetClassRoom = classRoom
            },
            new Announcement
            {
                Title = "School Holiday Notice",
                Category = "Notice",
                Date = new DateTime(2026, 8, 3),
                ShortDescription = "The school will remain closed on the announced holiday.",
                FullDescription = "The school administration has announced a holiday. Regular classes will resume according to the school timetable.",
                IssuedBy = "School Administration",
                IsImportant = false
            }
        );
    }

    private static void SeedCourseContent(AppDbContext db, ClassSubject mathematics, Student student)
    {
        // Gives the new teacher Materials/Remarks pages something real to
        // show on first use, instead of every one of them starting empty.
        db.CourseMaterials.Add(new CourseMaterial
        {
            ClassSubject = mathematics,
            Type = MaterialType.Assignment,
            Title = "Fractions Worksheet",
            Description = "Complete questions 1-20 on adding and subtracting fractions.",
            DueDate = new DateOnly(2026, 9, 20),
            PostedAt = new DateTime(2026, 9, 10)
        });

        db.CourseMaterials.Add(new CourseMaterial
        {
            ClassSubject = mathematics,
            Type = MaterialType.Handout,
            Title = "Multiplication Tables Reference Sheet",
            Description = "Keep this handy while practicing the worksheet.",
            PostedAt = new DateTime(2026, 9, 8)
        });

        db.TeacherRemarks.Add(new TeacherRemark
        {
            Student = student,
            ClassSubject = mathematics,
            Remark = "Ali has shown strong improvement in problem-solving speed this term.",
            CreatedAt = new DateTime(2026, 9, 12)
        });
    }
}
