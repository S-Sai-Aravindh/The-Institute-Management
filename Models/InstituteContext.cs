using Microsoft.EntityFrameworkCore;
using static Institute_Management.Models.TeacherModule;

namespace Institute_Management.Models
{
    public class InstituteContext : DbContext
    {
        public InstituteContext(DbContextOptions<InstituteContext> options) : base(options) { }

        public DbSet<UserModule.User> Users { get; set; }
        public DbSet<StudentModule.Student> Students { get; set; }
        public DbSet<TeacherModule.Teacher> Teachers { get; set; }
        public DbSet<AdminModule.Admin> Admins { get; set; }
        public DbSet<BatchModule.Batch> Batches { get; set; }
        public DbSet<CourseModule.Course> Courses { get; set; }
        public DbSet<StudentCourseModule.StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>()
           .HasMany(t => t.Courses)
           .WithOne(c => c.Teacher);

            // Configure composite primary key for StudentCourse
            modelBuilder.Entity<StudentCourseModule.StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Users
            modelBuilder.Entity<UserModule.User>().HasData(
                new UserModule.User { UserId = 1, Name = "Admin User", Email = "admin@example.com", Password = "admin123", Role = "Admin", ContactDetails = "123-456-7890" },
                new UserModule.User { UserId = 2, Name = "Student User", Email = "student@example.com", Password = "student123", Role = "Student", ContactDetails = "234-567-8901" },
                new UserModule.User { UserId = 3, Name = "Teacher User", Email = "teacher@example.com", Password = "teacher123", Role = "Teacher", ContactDetails = "345-678-9012" },
                new UserModule.User { UserId = 4, Name = "Student Two", Email = "student2@example.com", Password = "student456", Role = "Student", ContactDetails = "234-567-8902" },
                new UserModule.User { UserId = 5, Name = "Admin Two", Email = "admin2@example.com", Password = "admin456", Role = "Admin", ContactDetails = "123-456-7891" },
                new UserModule.User { UserId = 6, Name = "Teacher Two", Email = "teacher2@example.com", Password = "teacher456", Role = "Teacher", ContactDetails = "345-678-9013" }
            );

            // Admins
            modelBuilder.Entity<AdminModule.Admin>().HasData(
                new AdminModule.Admin { AdminId = 1, UserId = 1 },
                new AdminModule.Admin { AdminId = 2, UserId = 5 }
            );

            // Students
            modelBuilder.Entity<StudentModule.Student>().HasData(
                new StudentModule.Student { StudentId = 1, UserId = 2, BatchId = 1 },
                new StudentModule.Student { StudentId = 2, UserId = 4, BatchId = 2 }
            );

            // Teachers
            modelBuilder.Entity<TeacherModule.Teacher>().HasData(
                new TeacherModule.Teacher { TeacherId = 1, UserId = 3, SubjectSpecialization = "Mathematics" },
                new TeacherModule.Teacher { TeacherId = 2, UserId = 6, SubjectSpecialization = "Science" }
            );

            // Batches
            modelBuilder.Entity<BatchModule.Batch>().HasData(
                new BatchModule.Batch { BatchId = 1, BatchName = "Batch A", BatchTiming = "9:00 AM - 11:00 AM", BatchType = "Morning", CourseId = 1 },
                new BatchModule.Batch { BatchId = 2, BatchName = "Batch B", BatchTiming = "2:00 PM - 4:00 PM", BatchType = "Afternoon", CourseId = 2 }
            );

            // Courses
            modelBuilder.Entity<CourseModule.Course>().HasData(
                new CourseModule.Course { CourseId = 1, CourseName = "Algebra 101", Description = "Introduction to Algebra", TeacherId = 1 },
                new CourseModule.Course { CourseId = 2, CourseName = "Physics 101", Description = "Basic concepts of Physics", TeacherId = 2 }
            );

            // Student Courses
            modelBuilder.Entity<StudentCourseModule.StudentCourse>().HasData(
                new StudentCourseModule.StudentCourse { StudentId = 1, CourseId = 1 },
                new StudentCourseModule.StudentCourse { StudentId = 2, CourseId = 2 }
            );
        }

    }
}