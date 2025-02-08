﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Institute_Management.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Institute_Management.Services;
using static Institute_Management.Models.UserModule;
using Institute_Management.DTOs;

namespace Institute_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly InstituteContext _context;
        private readonly JwtService _jwtService;

        public AuthController(InstituteContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModule.User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> AuthenticateUser([FromQuery] string email, [FromQuery] string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { message = "Email and Password are required." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (user.Password != password) // Note: In a real application, always hash passwords
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var userDto = new UserDTO
            {
                UserId = (int)user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                ContactDetails = user.ContactDetails
            };

            return Ok(new
            {
                message = "Login successful",
                User = userDto
            });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email and Password are required." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (user.Password != request.Password)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var token = _jwtService.GenerateToken(user);
            return Ok(new { message = "Login successful", token });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("admin-data")]
        public IActionResult GetAdminData()
        {
            return Ok(new { message = "This is admin-only data" });
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpGet("teacher-data")]
        public IActionResult GetTeacherData()
        {
            return Ok(new { message = "This is teacher and admin accessible data" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest(new { message = "Invalid input. Email and Password are required." });
            }

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return Conflict(new { message = "User already exists." });
            }

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Register), new { id = user.UserId }, new { message = "Registration successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing your request.",
                    error = ex.Message,
                    suggestion = "Ensure the API URL is correct and the server is running."
                });
            }
        }


    }
}
