
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static Institute_Management.Models.CourseModule;

namespace Institute_Management.Models
{
    public class BatchModule
    {
        public class Batch
        {
            [Key]
            public int BatchId { get; set; }
            public string BatchName { get; set; }
            public string BatchTiming { get; set; }
            public string BatchType { get; set; }
            public int? CourseId { get; set; }

            [ForeignKey("CourseId")]
            public CourseModule.Course Course { get; set; }

            public ICollection<Course> Courses { get; set; }
        }
    }
}
