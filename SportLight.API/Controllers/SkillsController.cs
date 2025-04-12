using Microsoft.AspNetCore.Mvc;
using SportLight.API.Data;
using SportLight.API.DTOs;
using SportLight.API.Models;
using Microsoft.EntityFrameworkCore;

namespace SportLight.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Skill>>>> GetSkills(
            [FromQuery] string? sport = null,
            [FromQuery] string? difficulty = null,
            [FromQuery] string? category = null,
            [FromQuery] bool? isPublished = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Skills
                    .Include(s => s.Instructor)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(sport))
                    query = query.Where(s => s.Sport == sport);

                if (!string.IsNullOrEmpty(difficulty))
                    query = query.Where(s => s.Difficulty == difficulty);

                if (!string.IsNullOrEmpty(category))
                    query = query.Where(s => s.Category == category);

                if (isPublished.HasValue)
                    query = query.Where(s => s.IsPublished == isPublished.Value);

                var totalCount = await query.CountAsync();
                var skills = await query
                    .OrderByDescending(s => s.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new ApiResponse<List<Skill>>
                {
                    Success = true,
                    Data = skills,
                    Message = $"Retrieved {skills.Count} skills"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<Skill>>
                {
                    Success = false,
                    Error = "An error occurred while retrieving skills"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Skill>>> GetSkill(string id)
        {
            try
            {
                var skill = await _context.Skills
                    .Include(s => s.Instructor)
                    .Include(s => s.Reviews)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (skill == null)
                {
                    return NotFound(new ApiResponse<Skill>
                    {
                        Success = false,
                        Error = "Skill not found"
                    });
                }

                return Ok(new ApiResponse<Skill>
                {
                    Success = true,
                    Data = skill
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Skill>
                {
                    Success = false,
                    Error = "An error occurred while retrieving the skill"
                });
            }
        }

        [HttpGet("sports")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetSports()
        {
            try
            {
                var sports = await _context.Skills
                    .Where(s => s.IsPublished)
                    .Select(s => s.Sport)
                    .Distinct()
                    .ToListAsync();

                return Ok(new ApiResponse<List<string>>
                {
                    Success = true,
                    Data = sports
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<string>>
                {
                    Success = false,
                    Error = "An error occurred while retrieving sports"
                });
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetCategories()
        {
            try
            {
                var categories = await _context.Skills
                    .Where(s => s.IsPublished)
                    .Select(s => s.Category)
                    .Distinct()
                    .ToListAsync();

                return Ok(new ApiResponse<List<string>>
                {
                    Success = true,
                    Data = categories
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<string>>
                {
                    Success = false,
                    Error = "An error occurred while retrieving categories"
                });
            }
        }
    }
} 