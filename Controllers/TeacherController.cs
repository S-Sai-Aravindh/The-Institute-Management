using Institute_Management.DTOs;
using Institute_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Institute_Management.Models.StudentModule;
using static Institute_Management.Models.TeacherModule;

namespace Institute_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly InstituteContext _context;

        public TeacherController(InstituteContext context)
        {
            _context = context;
        }

        // GET: api/teacher
        [HttpGet]
        public async Task<IActionResult> GetTeachers()
        {
            // Fetch all teachers along with their related courses
            var teachers = await _context.Teachers
                .Include(t => t.Courses) // Assuming a Teacher has a collection of Courses
                .Include(t => t.User)
                .ToListAsync();


            // Create a list of TeacherDTOs
            var teacherDtos = teachers.Select(t => new TeacherDTO
            {
                TeacherId = t.TeacherId,
                User = new UserDTO
                {
                    UserId = t.User.UserId,
                    Name = t.User.Name,
                    Email = t.User.Email,
                    Role = t.User.Role,
                    ContactDetails = t.User.ContactDetails
                },
                Courses = t.Courses.Select(c => new CourseDTO
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    //Teacher = c.Teacher,
                }).ToList()
            }).ToList();

            return Ok(teacherDtos);
        }

        // GET: api/teacher/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeacher(int id)
        {
            // Fetch the teacher along with their related courses
            var teacher = await _context.Teachers
                .Include(t => t.Courses) // Assuming a Teacher has a collection of Courses
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TeacherId == id);

            if (teacher == null) return NotFound();

            // Create a TeacherDTO
            var teacherDto = new TeacherDTO
            {
                TeacherId = teacher.TeacherId,
                User = new UserDTO
                {
                    UserId = teacher.UserId,
                    Name = teacher.User.Name,
                    Email = teacher.User.Email,
                    Role = teacher.User.Role,
                    ContactDetails = teacher.User.ContactDetails
                },
                Courses = teacher.Courses.Select(c => new CourseDTO
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    //Teacher = c.Teacher,
                }).ToList()
            
            //Name = teacher.Name,
            //    Email = teacher.Email,
            //    PhoneNumber = teacher.PhoneNumber,
            //    Address = teacher.Address,
            //    Courses = teacher.Courses.Select(c => new CourseDto
            //    {
            //        Id = c.Id,
            //        CourseName = c.CourseName,
            //        Description = c.Description,
            //        TeacherId = c.TeacherId,
            //    }).ToList()
            };

            return Ok(teacherDto);
        }

        // POST: api/teacher
        [HttpPost]
        public async Task<ActionResult<Teacher>> PostTeacher(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeacher", new { id = teacher.TeacherId }, teacher);
        }

        // PUT: api/teacher/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(int id, Teacher teacher)
        {
            if (id != teacher.TeacherId)
            {
                return BadRequest();
            }

            _context.Entry(teacher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/teacher/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.TeacherId == id);
        }
    }
}
