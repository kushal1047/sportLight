using Microsoft.AspNetCore.Mvc;
using SportLight.API.Data;
using SportLight.API.DTOs;
using SportLight.API.Models;
using SportLight.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace SportLight.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest request)
        {
            try
            {
                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest(new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        Error = "User with this email already exists"
                    });
                }

                if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                {
                    return BadRequest(new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        Error = "Username is already taken"
                    });
                }

                // Create new user
                var user = new User
                {
                    Email = request.Email,
                    Username = request.Username,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    SkillLevel = request.SkillLevel,
                    Bio = request.Bio,
                    SportsInterests = request.SportsInterests,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate token
                var token = _jwtService.GenerateToken(user);

                var response = new AuthResponse
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    SkillLevel = user.SkillLevel,
                    ExpiresAt = DateTime.UtcNow.AddDays(7)
                };

                return Ok(new ApiResponse<AuthResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "User registered successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Error = "An error occurred during registration"
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request)
        {
            try
            {
                // For now, we'll use a simple authentication
                // In a real app, you'd hash passwords and verify them
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                {
                    return BadRequest(new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        Error = "Invalid email or password"
                    });
                }

                // Generate token
                var token = _jwtService.GenerateToken(user);

                var response = new AuthResponse
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    SkillLevel = user.SkillLevel,
                    ExpiresAt = DateTime.UtcNow.AddDays(7)
                };

                return Ok(new ApiResponse<AuthResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Error = "An error occurred during login"
                });
            }
        }
    }
} 