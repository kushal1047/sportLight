using Microsoft.AspNetCore.Mvc;
using SportLight.API.Data;
using SportLight.API.DTOs;
using SportLight.API.Models;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<User>>>> GetUsers(
            [FromQuery] string? skillLevel = null,
            [FromQuery] string? sport = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                if (!string.IsNullOrEmpty(skillLevel))
                    query = query.Where(u => u.SkillLevel == skillLevel);

                if (!string.IsNullOrEmpty(sport))
                    query = query.Where(u => u.SportsInterests.Contains(sport));

                var totalCount = await query.CountAsync();
                var users = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new ApiResponse<List<User>>
                {
                    Success = true,
                    Data = users,
                    Message = $"Retrieved {users.Count} users"
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<User>>> GetUser(string id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Skills)
                    .Include(u => u.InstructorSessions)
                    .Include(u => u.StudentSessions)
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
                    Error = "An error occurred while retrieving the user"
                });
            }
        }

        [HttpGet("instructors")]
        public async Task<ActionResult<ApiResponse<List<User>>>> GetInstructors(
            [FromQuery] string? sport = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Users
                    .Where(u => u.Skills.Any(s => s.IsPublished))
                    .Include(u => u.Skills.Where(s => s.IsPublished))
                    .AsQueryable();

                if (!string.IsNullOrEmpty(sport))
                    query = query.Where(u => u.Skills.Any(s => s.Sport == sport && s.IsPublished));

                var totalCount = await query.CountAsync();
                var instructors = await query
                    .OrderByDescending(u => u.Skills.Count)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new ApiResponse<List<User>>
                {
                    Success = true,
                    Data = instructors,
                    Message = $"Retrieved {instructors.Count} instructors"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<User>>
                {
                    Success = false,
                    Error = "An error occurred while retrieving instructors"
                });
            }
        }

        [HttpGet("skill-levels")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetSkillLevels()
        {
            try
            {
                var skillLevels = await _context.Users
                    .Select(u => u.SkillLevel)
                    .Distinct()
                    .ToListAsync();

                return Ok(new ApiResponse<List<string>>
                {
                    Success = true,
                    Data = skillLevels
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<string>>
                {
                    Success = false,
                    Error = "An error occurred while retrieving skill levels"
                });
            }
        }
    }
} 