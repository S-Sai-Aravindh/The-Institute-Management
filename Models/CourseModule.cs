using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Institute_Management.Models
{
    public class CourseModule
    {
        public class Course
        {
            [Key]
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public string Description { get; set; }
            public int? TeacherId { get; set; }

            [ForeignKey("TeacherId")]
            public TeacherModule.Teacher Teacher { get; set; }
            public List<StudentCourseModule.StudentCourse> Enrollments { get; set; }
        }
    }
}
