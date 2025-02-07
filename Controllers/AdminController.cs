using Institute_Management.DTOs;
using Institute_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Institute_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly InstituteContext _context;

        public AdminController(InstituteContext context)
        {
            _context = context;
        }

        #region Students Management

        // GET: api/admin/students
        [HttpGet("students")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudents()
        {
            var students = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Batch)
                    .ThenInclude(b => b.Courses) // Include Courses in Batch
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Teacher)
                            .ThenInclude(t => t.User)
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
                    Batch = new BatchDTO
                    {
                        BatchName = s.Batch.BatchName,
                        BatchTiming = s.Batch.BatchTiming,
                        BatchType = s.Batch.BatchType,
                        Course = new CourseDTO
                        {
                            CourseId = (int)s.Batch.CourseId,
                            CourseName = s.Batch.Course.CourseName,
                            Description = s.Batch.Course.Description,
                            Teacher = new TeacherDTO
                            {
                                TeacherId = (int)s.Batch.Course.TeacherId,
                                UserId = s.UserId,
                                SubjectSpecialization = s.Batch.Course.Teacher.SubjectSpecialization
                            }
                        }
                    },
                    Enrollments = s.Enrollments.Select(e => new EnrollmentDTO
                    {
                        StudentId = e.StudentId,
                        CourseId = e.CourseId,
                        Course = new CourseDTO
                        {
                            CourseId = e.Course.CourseId,
                            CourseName = e.Course.CourseName,
                            Description = e.Course.Description,
                            Teacher = e.Course.Teacher != null ? new TeacherDTO
                            {
                                TeacherId = e.Course.Teacher.TeacherId,
                                UserId = e.Course.Teacher.UserId,
                                SubjectSpecialization = e.Course.Teacher.SubjectSpecialization,
                                User = new UserDTO
                                {
                                    UserId = e.Course.Teacher.User.UserId,
                                    Name = e.Course.Teacher.User.Name,
                                    Email = e.Course.Teacher.User.Email,
                                    Role = e.Course.Teacher.User.Role,
                                    ContactDetails = e.Course.Teacher.User.ContactDetails
                                }
                            } : null
                        }
                    }).ToList()
                })
                .ToListAsync();

            return Ok(students);
        }



        [HttpGet("students/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Batch)
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null) return NotFound();

            var studentDto = new StudentDTO
            {
                StudentId = student.StudentId,
                UserId = student.UserId,
                BatchId = student.BatchId,
                User = student.User != null ? new UserDTO
                {
                    UserId = student.User.UserId,
                    Name = student.User.Name,
                    Email = student.User.Email,
                    Role = student.User.Role,
                    ContactDetails = student.User.ContactDetails
                } : null,
                Batch = student.Batch != null ? new BatchDTO
                {
                    BatchId = student.Batch.BatchId,
                    BatchName = student.Batch.BatchName,
                    BatchTiming = student.Batch.BatchTiming,
                    BatchType = student.Batch.BatchType,
                    Course = student.Batch.Course != null ? new CourseDTO
                    {
                        CourseId = student.Batch.Course.CourseId,
                        CourseName = student.Batch.Course.CourseName,
                        Description = student.Batch.Course.Description
                    } : null
                } : null,
                Enrollments = student.Enrollments.Select(e => new EnrollmentDTO
                {
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    Course = e.Course != null ? new CourseDTO
                    {
                        CourseId = e.Course.CourseId,
                        CourseName = e.Course.CourseName,
                        Description = e.Course.Description,
                        Teacher = e.Course.Teacher != null ? new TeacherDTO
                        {
                            TeacherId = e.Course.Teacher.TeacherId,
                            UserId = e.Course.Teacher.UserId,
                            SubjectSpecialization = e.Course.Teacher.SubjectSpecialization
                        } : null
                    } : null
                }).ToList()
            };


            return Ok(studentDto);
        }


        // POST: api/admin/students
        [HttpPost("students")]
        public async Task<ActionResult<StudentDTO>> CreateStudent([FromBody] StudentDTO studentDto)
        {
            var student = new StudentModule.Student
            {
                UserId = studentDto.UserId,
                BatchId = studentDto.BatchId
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllStudents), new { id = student.StudentId }, studentDto);
        }

        // PUT: api/admin/students/{id}
        [HttpPut("students/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDTO studentDto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            student.UserId = studentDto.UserId;
            student.BatchId = studentDto.BatchId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/admin/students/{id}
        [HttpDelete("students/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region Teacher Management

        // GET: api/admin/teachers
        [HttpGet("teachers")]
        public async Task<ActionResult<IEnumerable<TeacherDTO>>> GetAllTeachers()
        {
            var teachers = await _context.Teachers
                .Include(t => t.User)
                .Select(t => new TeacherDTO
                {
                    TeacherId = t.TeacherId,
                    UserId = t.UserId,
                    SubjectSpecialization = t.SubjectSpecialization,
                    User = new UserDTO
                    {
                        UserId = t.User.UserId,
                        Name = t.User.Name,
                        Email = t.User.Email,
                        Role = t.User.Role,
                        ContactDetails = t.User.ContactDetails
                    }
                })
                .ToListAsync();

            return Ok(teachers);
        }

        // POST: api/admin/teachers
        [HttpPost("teachers")]
        public async Task<ActionResult<TeacherDTO>> CreateTeacher([FromBody] TeacherDTO teacherDto)
        {
            var teacher = new TeacherModule.Teacher
            {
                UserId = teacherDto.UserId,
                SubjectSpecialization = teacherDto.SubjectSpecialization
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllTeachers), new { id = teacher.TeacherId }, teacherDto);
        }

        // PUT: api/admin/teachers/{id}
        [HttpPut("teachers/{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] TeacherDTO teacherDto)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            teacher.UserId = teacherDto.UserId;
            teacher.SubjectSpecialization = teacherDto.SubjectSpecialization;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/admin/teachers/{id}
        [HttpDelete("teachers/{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region Course Management

        // GET: api/admin/courses
        [HttpGet("courses")]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetAllCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Teacher)
                .ThenInclude(t => t.User)
                .Select(c => new CourseDTO
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    //TeacherId = c.TeacherId,
                    Teacher = new TeacherDTO
                    {
                        TeacherId = c.Teacher.TeacherId,
                        UserId = c.Teacher.UserId,
                        SubjectSpecialization = c.Teacher.SubjectSpecialization,
                        User = new UserDTO
                        {
                            UserId = c.Teacher.User.UserId,
                            Name = c.Teacher.User.Name,
                            Email = c.Teacher.User.Email,
                            Role = c.Teacher.User.Role,
                            ContactDetails = c.Teacher.User.ContactDetails,
                        }
                    }
                })
                .ToListAsync();

            return Ok(courses);
        }

        // POST: api/admin/courses
        [HttpPost("courses")]
        public async Task<ActionResult<CourseDTO>> CreateCourse([FromBody] CourseDTO courseDto)
        {
            var course = new CourseModule.Course
            {
                CourseName = courseDto.CourseName,
                Description = courseDto.Description,
                //TeacherId = courseDto.TeacherId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllCourses), new { id = course.CourseId }, courseDto);
        }

        // PUT: api/admin/courses/{id}
        [HttpPut("courses/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDTO courseDto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            course.CourseName = courseDto.CourseName;
            course.Description = courseDto.Description;
            //course.TeacherId = courseDto.TeacherId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/admin/courses/{id}
        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region Batch Management    

        // GET: api/admin/batches
        [HttpGet("batches")]
        public async Task<ActionResult<IEnumerable<BatchDTO>>> GetAllBatches()
        {
            var batches = await _context.Batches
                .Include(b => b.Course)
                .ThenInclude(c => c.Teacher) // Ensure Teacher is included
                .ThenInclude(t => t.User) // Include User details inside Teacher
                .Select(b => new BatchDTO
                {
                    BatchId = b.BatchId,
                    BatchName = b.BatchName,
                    BatchTiming = b.BatchTiming,
                    BatchType = b.BatchType,
                    Course = new CourseDTO
                    {
                        CourseId = b.Course.CourseId,
                        CourseName = b.Course.CourseName,
                        Description = b.Course.Description,
                        Teacher = new TeacherDTO
                        {
                            TeacherId = b.Course.Teacher.TeacherId,
                            UserId = b.Course.Teacher.UserId,
                            SubjectSpecialization = b.Course.Teacher.SubjectSpecialization,
                            User = new UserDTO
                            {
                                UserId = b.Course.Teacher.User.UserId,
                                Name = b.Course.Teacher.User.Name,
                                Email = b.Course.Teacher.User.Email,
                                Role = b.Course.Teacher.User.Role,
                                ContactDetails = b.Course.Teacher.User.ContactDetails
                            }
                        }
                    }
                })
                .ToListAsync();

            return Ok(batches);
        }

        // POST: api/admin/batches
        [HttpPost("batches")]
        public async Task<ActionResult<BatchDTO>> CreateBatch([FromBody] BatchDTO batchDto)
        {
            var batch = new BatchModule.Batch
            {
                BatchName = batchDto.BatchName,
                BatchTiming = batchDto.BatchTiming,
                BatchType = batchDto.BatchType,
                //CourseId = batchDto.CourseId
            };

            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllBatches), new { id = batch.BatchId }, batchDto);
        }

        // PUT: api/admin/batches/{id}
        [HttpPut("batches/{id}")]
        public async Task<IActionResult> UpdateBatch(int id, [FromBody] BatchDTO batchDto)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null) return NotFound();

            batch.BatchName = batchDto.BatchName;
            batch.BatchTiming = batchDto.BatchTiming;
            batch.BatchType = batchDto.BatchType;
            //batch.CourseId = batchDto.CourseId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/admin/batches/{id}
        [HttpDelete("batches/{id}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null) return NotFound();

            _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region Reporting

        // GET: api/admin/reports/students
        [HttpGet("reports/students")]
        public async Task<IActionResult> GetStudentReports()
        {
            // This is a simplified view of student reports, you can enhance it as per your requirement.
            var studentReports = await _context.Students
                .Select(s => new
                {
                    StudentId = s.StudentId,
                    UserName = s.User.Name,
                    BatchName = s.Batch.BatchName,
                    EnrolledCourses = _context.StudentCourses.Count(sc => sc.StudentId == s.StudentId)
                })
                .ToListAsync();

            return Ok(studentReports);
        }
        #endregion
    }
}

