using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Institute_Management.Models
{
    public class StudentModule
    {
        public class Student
        {
            [Key]
            public int StudentId { get; set; }
            public int UserId { get; set; }
            public int? BatchId { get; set; }

            [ForeignKey("UserId")]
            public UserModule.User User { get; set; }

            [ForeignKey("BatchId")]
            public BatchModule.Batch Batch { get; set; }
            public List<StudentCourseModule.StudentCourse> Enrollments { get; set; }
        }
    }
}
