using Institute_Management.DTOs;
using Institute_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Institute_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly InstituteContext _context;

        public StudentController(InstituteContext context)
        {
            _context = context;
        }
        //// http: //localhost:5109/api/student/students

        [HttpGet("students")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudents()
        {
            var students = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Batch)
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .Select(s => new StudentDTO
                {
                    StudentId = s.StudentId,
                    UserId = s.UserId,
                    BatchId = s.BatchId,
                    User = new UserDTO
                    {
                        UserId = s.User.UserId,
                        Name = s.User.Name,
                        Email = s.User.Email,
                        Role = s.User.Role,
                        ContactDetails = s.User.ContactDetails
                    },
                    Batch = s.Batch != null ? new BatchDTO
                    {
                        BatchId = s.Batch.BatchId,
                        BatchName = s.Batch.BatchName,
                        BatchTiming = s.Batch.BatchTiming,
                        BatchType = s.Batch.BatchType
                    } : null,
                    Enrollments = s.Enrollments.Select(e => new EnrollmentDTO
                    {
                        StudentId = e.StudentId,
                        CourseId = e.CourseId,
                        Course = new CourseDTO
                        {
                            CourseId = e.Course.CourseId,
                            CourseName = e.Course.CourseName,
                            Description = e.Course.Description
                        }
                    }).ToList()
                })
                .ToListAsync();

            return Ok(students);
        }




        // 1. Get Student Profile
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentProfile(int id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null) return NotFound();

            var studentDto = new StudentDTO
            {
                StudentId = student.StudentId,
                UserId = student.UserId,
                BatchId = student.BatchId,
                User = new UserDTO
                {
                    UserId = student.User.UserId,
                    Name = student.User.Name,
                    Email = student.User.Email,
                    Role = student.User.Role,
                    ContactDetails = student.User.ContactDetails
                },
                Batch = student.Batch != null ? new BatchDTO
                {
                    BatchId = student.Batch.BatchId,
                    BatchName = student.Batch.BatchName,
                    BatchTiming = student.Batch.BatchTiming,
                    BatchType = student.Batch.BatchType
                } : null,
                Enrollments = student.Enrollments.Select(e => new EnrollmentDTO
                {
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    Course = new CourseDTO
                    {
                        CourseId = e.Course.CourseId,
                        CourseName = e.Course.CourseName,
                        Description = e.Course.Description
                    }
                }).ToList()
            };

            return Ok(studentDto);
        }

        // 2. Update Student Profile
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudentProfile(int id, [FromBody] StudentDTO updatedStudentDto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            student.UserId = updatedStudentDto.UserId;
            student.BatchId = updatedStudentDto.BatchId;

            await _context.SaveChangesAsync();
            return Ok(updatedStudentDto);
        }

        // 3. Get Available Courses
        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses
                .Select(c => new CourseDTO
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description
                })
                .ToListAsync();

            return Ok(courses);
        }

        // 4. Enroll in a Course
        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollInCourse([FromBody] EnrollmentDTO enrollmentDto)
        {
            var enrollment = new StudentCourseModule.StudentCourse
            {
                StudentId = enrollmentDto.StudentId,
                CourseId = enrollmentDto.CourseId
            };

            _context.StudentCourses.Add(enrollment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Enrollment successful" });
        }

        // 5. Get Student's Assigned Batches
        [HttpGet("{studentId}/batches")]
        public async Task<IActionResult> GetStudentBatches(int studentId)
        {
            var batches = await _context.Batches
                .Where(b => _context.StudentCourses.Any(sc => sc.StudentId == studentId && sc.CourseId == b.CourseId))
                .Select(b => new BatchDTO
                {
                    BatchId = b.BatchId,
                    BatchName = b.BatchName,
                    BatchTiming = b.BatchTiming,
                    BatchType = b.BatchType
                })
                .ToListAsync();

            return Ok(batches);
        }
    }
}
