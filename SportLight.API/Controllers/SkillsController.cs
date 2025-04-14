using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportLight.API.Data;
using SportLight.API.DTOs;
using SportLight.API.Models;

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
            [FromQuery] string? category = null,
            [FromQuery] string? difficulty = null,
            [FromQuery] string? instructorId = null,
            [FromQuery] bool? isPublished = null)
        {
            try
            {
                var query = _context.Skills
                    .Include(s => s.Instructor)
                    .Include(s => s.Reviews)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(sport))
                    query = query.Where(s => s.Sport == sport);

                if (!string.IsNullOrEmpty(category))
                    query = query.Where(s => s.Category == category);

                if (!string.IsNullOrEmpty(difficulty))
                    query = query.Where(s => s.Difficulty == difficulty);

                if (!string.IsNullOrEmpty(instructorId))
                    query = query.Where(s => s.InstructorId == instructorId);

                if (isPublished.HasValue)
                    query = query.Where(s => s.IsPublished == isPublished.Value);

                var skills = await query
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return Ok(new ApiResponse<List<Skill>>
                {
                    Success = true,
                    Data = skills
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
                    Error = "An error occurred while retrieving skill"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Skill>>> CreateSkill([FromBody] CreateSkillRequest request)
        {
            try
            {
                // Verify instructor exists
                var instructor = await _context.Users.FindAsync(request.InstructorId);
                if (instructor == null)
                {
                    return BadRequest(new ApiResponse<Skill>
                    {
                        Success = false,
                        Error = "Instructor not found"
                    });
                }

                var skill = new Skill
                {
                    Title = request.Title,
                    Description = request.Description,
                    Category = request.Category,
                    Sport = request.Sport,
                    Difficulty = request.Difficulty,
                    InstructorId = request.InstructorId,
                    VideoUrl = request.VideoUrl,
                    ThumbnailUrl = request.ThumbnailUrl,
                    Duration = request.Duration,
                    Price = request.Price,
                    IsPublished = request.IsPublished,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Skills.Add(skill);
                await _context.SaveChangesAsync();

                // Reload with instructor data
                await _context.Entry(skill).Reference(s => s.Instructor).LoadAsync();

                return CreatedAtAction(nameof(GetSkill), new { id = skill.Id }, new ApiResponse<Skill>
                {
                    Success = true,
                    Data = skill,
                    Message = "Skill created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Skill>
                {
                    Success = false,
                    Error = "An error occurred while creating skill"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Skill>>> UpdateSkill(string id, [FromBody] UpdateSkillRequest request)
        {
            try
            {
                var skill = await _context.Skills.FindAsync(id);

                if (skill == null)
                {
                    return NotFound(new ApiResponse<Skill>
                    {
                        Success = false,
                        Error = "Skill not found"
                    });
                }

                // Update skill properties
                if (!string.IsNullOrEmpty(request.Title))
                    skill.Title = request.Title;

                if (!string.IsNullOrEmpty(request.Description))
                    skill.Description = request.Description;

                if (!string.IsNullOrEmpty(request.Category))
                    skill.Category = request.Category;

                if (!string.IsNullOrEmpty(request.Sport))
                    skill.Sport = request.Sport;

                if (!string.IsNullOrEmpty(request.Difficulty))
                    skill.Difficulty = request.Difficulty;

                if (!string.IsNullOrEmpty(request.VideoUrl))
                    skill.VideoUrl = request.VideoUrl;

                if (!string.IsNullOrEmpty(request.ThumbnailUrl))
                    skill.ThumbnailUrl = request.ThumbnailUrl;

                if (request.Duration.HasValue)
                    skill.Duration = request.Duration.Value;

                if (request.Price.HasValue)
                    skill.Price = request.Price.Value;

                if (request.IsPublished.HasValue)
                    skill.IsPublished = request.IsPublished.Value;

                skill.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Reload with instructor data
                await _context.Entry(skill).Reference(s => s.Instructor).LoadAsync();

                return Ok(new ApiResponse<Skill>
                {
                    Success = true,
                    Data = skill,
                    Message = "Skill updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Skill>
                {
                    Success = false,
                    Error = "An error occurred while updating skill"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSkill(string id)
        {
            try
            {
                var skill = await _context.Skills.FindAsync(id);

                if (skill == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Error = "Skill not found"
                    });
                }

                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Skill deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Error = "An error occurred while deleting skill"
                });
            }
        }

        [HttpGet("sports")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetSports()
        {
            try
            {
                var sports = await _context.Skills
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

    public class CreateSkillRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Sport { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "beginner";
        public string InstructorId { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int Duration { get; set; } = 0;
        public decimal Price { get; set; } = 0.00m;
        public bool IsPublished { get; set; } = false;
    }

    public class UpdateSkillRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Sport { get; set; }
        public string? Difficulty { get; set; }
        public string? VideoUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int? Duration { get; set; }
        public decimal? Price { get; set; }
        public bool? IsPublished { get; set; }
    }
} 