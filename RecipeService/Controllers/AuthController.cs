using Domain.DTOs;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RecipeApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // =======================================
        // ✅ POST: api/auth/register
        // =======================================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // 🔹 Basic validation
            if (dto == null || dto.User == null)
                return BadRequest("User data is missing.");

            if (string.IsNullOrWhiteSpace(dto.User.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.User.Email))
                return BadRequest("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required.");

            // 🔹 Try to register user
            var user = await _userService.RegisterAsync(dto.User, dto.Password);
            if (user == null)
                return Conflict("User already exists with this email or username.");

            return Ok(new
            {
                message = "User registered successfully.",
                user
            });
        }

        // =======================================
        // ✅ POST: api/auth/login
        // =======================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return BadRequest("Username and password are required.");

            var token = await _userService.LoginAsync(username, password);
            if (token == null)
                return Unauthorized("Invalid username or password.");

            return Ok(new
            {
                message = "Login successful.",
                token
            });
        }

        // =======================================
        // ✅ GET: api/auth/users
        // =======================================
        [HttpGet("users")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            if (users == null || !users.Any())
                return NotFound("No users found.");

            return Ok(users);
        }

        // =======================================
        // ✅ GET: api/auth/{id}
        // =======================================
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }
    }
}
