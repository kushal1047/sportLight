using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportLight.API.Data;
using SportLight.API.DTOs;
using SportLight.API.Models;

namespace SportLight.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<User>>> GetUser(string id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Skills)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<User>
                    {
                        Success = false,
                        Error = "User not found"
                    });
                }

                return Ok(new ApiResponse<User>
                {
                    Success = true,
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<User>
                {
                    Success = false,
                    Error = "An error occurred while retrieving user"
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<User>>>> GetUsers([FromQuery] string? sport = null, [FromQuery] string? skillLevel = null)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                if (!string.IsNullOrEmpty(sport))
                {
                    query = query.Where(u => u.SportsInterests.Contains(sport));
                }

                if (!string.IsNullOrEmpty(skillLevel))
                {
                    query = query.Where(u => u.SkillLevel == skillLevel);
                }

                var users = await query
                    .Include(u => u.Skills.Where(s => s.IsPublished))
                    .ToListAsync();

                return Ok(new ApiResponse<List<User>>
                {
                    Success = true,
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<User>>
                {
                    Success = false,
                    Error = "An error occurred while retrieving users"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<User>>> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<User>
                    {
                        Success = false,
                        Error = "User not found"
                    });
                }

                // Update user properties
                if (!string.IsNullOrEmpty(request.FirstName))
                    user.FirstName = request.FirstName;
                
                if (!string.IsNullOrEmpty(request.LastName))
                    user.LastName = request.LastName;
                
                if (!string.IsNullOrEmpty(request.Bio))
                    user.Bio = request.Bio;
                
                if (!string.IsNullOrEmpty(request.SkillLevel))
                    user.SkillLevel = request.SkillLevel;
                
                if (!string.IsNullOrEmpty(request.SportsInterests))
                    user.SportsInterests = request.SportsInterests;
                
                if (!string.IsNullOrEmpty(request.Avatar))
                    user.Avatar = request.Avatar;

                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<User>
                {
                    Success = true,
                    Data = user,
                    Message = "User updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<User>
                {
                    Success = false,
                    Error = "An error occurred while updating user"
                });
            }
        }
    }

    public class UpdateUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? SkillLevel { get; set; }
        public string? SportsInterests { get; set; }
        public string? Avatar { get; set; }
    }
} 