using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static Institute_Management.Models.CourseModule;

namespace Institute_Management.Models
{
    public class TeacherModule
    {
        public class Teacher
        {
            [Key]
            public int TeacherId { get; set; }
            public int UserId { get; set; }
            public string SubjectSpecialization { get; set; }

            [ForeignKey("UserId")]
            public UserModule.User User { get; set; }

            public ICollection<Course> Courses { get; set; }
        }
    }
}
