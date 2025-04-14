using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportLight.API.Data;
using SportLight.API.DTOs;
using SportLight.API.Models;

namespace SportLight.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Session>>>> GetSessions(
            [FromQuery] string? skillId = null,
            [FromQuery] string? instructorId = null,
            [FromQuery] string? studentId = null,
            [FromQuery] string? status = null)
        {
            try
            {
                var query = _context.Sessions
                    .Include(s => s.Skill)
                    .Include(s => s.Instructor)
                    .Include(s => s.Student)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(skillId))
                    query = query.Where(s => s.SkillId == skillId);

                if (!string.IsNullOrEmpty(instructorId))
                    query = query.Where(s => s.InstructorId == instructorId);

                if (!string.IsNullOrEmpty(studentId))
                    query = query.Where(s => s.StudentId == studentId);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(s => s.Status == status);

                var sessions = await query
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return Ok(new ApiResponse<List<Session>>
                {
                    Success = true,
                    Data = sessions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<Session>>
                {
                    Success = false,
                    Error = "An error occurred while retrieving sessions"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Session>>> GetSession(string id)
        {
            try
            {
                var session = await _context.Sessions
                    .Include(s => s.Skill)
                    .Include(s => s.Instructor)
                    .Include(s => s.Student)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (session == null)
                {
                    return NotFound(new ApiResponse<Session>
                    {
                        Success = false,
                        Error = "Session not found"
                    });
                }

                return Ok(new ApiResponse<Session>
                {
                    Success = true,
                    Data = session
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Session>
                {
                    Success = false,
                    Error = "An error occurred while retrieving session"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Session>>> CreateSession([FromBody] CreateSessionRequest request)
        {
            try
            {
                // Verify skill exists
                var skill = await _context.Skills.FindAsync(request.SkillId);
                if (skill == null)
                {
                    return BadRequest(new ApiResponse<Session>
                    {
                        Success = false,
                        Error = "Skill not found"
                    });
                }

                // Verify instructor exists
                var instructor = await _context.Users.FindAsync(request.InstructorId);
                if (instructor == null)
                {
                    return BadRequest(new ApiResponse<Session>
                    {
                        Success = false,
                        Error = "Instructor not found"
                    });
                }

                // Verify student exists
                var student = await _context.Users.FindAsync(request.StudentId);
                if (student == null)
                {
                    return BadRequest(new ApiResponse<Session>
                    {
                        Success = false,
                        Error = "Student not found"
                    });
                }

                var session = new Session
                {
                    SkillId = request.SkillId,
                    InstructorId = request.InstructorId,
                    StudentId = request.StudentId,
                    Status = "scheduled",
                    ScheduledAt = request.ScheduledAt,
                    Duration = request.Duration,
                    Price = request.Price,
                    Notes = request.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();

                // Reload with related data
                await _context.Entry(session).Reference(s => s.Skill).LoadAsync();
                await _context.Entry(session).Reference(s => s.Instructor).LoadAsync();
                await _context.Entry(session).Reference(s => s.Student).LoadAsync();

                return CreatedAtAction(nameof(GetSession), new { id = session.Id }, new ApiResponse<Session>
                {
                    Success = true,
                    Data = session,
                    Message = "Session created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Session>
                {
                    Success = false,
                    Error = "An error occurred while creating session"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Session>>> UpdateSession(string id, [FromBody] UpdateSessionRequest request)
        {
            try
            {
                var session = await _context.Sessions.FindAsync(id);

                if (session == null)
                {
                    return NotFound(new ApiResponse<Session>
                    {
                        Success = false,
                        Error = "Session not found"
                    });
                }

                // Update session properties
                if (!string.IsNullOrEmpty(request.Status))
                    session.Status = request.Status;

                if (request.ScheduledAt.HasValue)
                    session.ScheduledAt = request.ScheduledAt.Value;

                if (request.Duration.HasValue)
                    session.Duration = request.Duration.Value;

                if (request.Price.HasValue)
                    session.Price = request.Price.Value;

                if (!string.IsNullOrEmpty(request.MeetingUrl))
                    session.MeetingUrl = request.MeetingUrl;

                if (!string.IsNullOrEmpty(request.Notes))
                    session.Notes = request.Notes;

                if (request.Rating.HasValue)
                    session.Rating = request.Rating.Value;

                if (!string.IsNullOrEmpty(request.Review))
                    session.Review = request.Review;

                session.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Reload with related data
                await _context.Entry(session).Reference(s => s.Skill).LoadAsync();
                await _context.Entry(session).Reference(s => s.Instructor).LoadAsync();
                await _context.Entry(session).Reference(s => s.Student).LoadAsync();

                return Ok(new ApiResponse<Session>
                {
                    Success = true,
                    Data = session,
                    Message = "Session updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Session>
                {
                    Success = false,
                    Error = "An error occurred while updating session"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSession(string id)
        {
            try
            {
                var session = await _context.Sessions.FindAsync(id);

                if (session == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Error = "Session not found"
                    });
                }

                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Session deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Error = "An error occurred while deleting session"
                });
            }
        }

        [HttpGet("statuses")]
        public ActionResult<ApiResponse<List<string>>> GetSessionStatuses()
        {
            try
            {
                var statuses = new List<string> { "scheduled", "in-progress", "completed", "cancelled" };

                return Ok(new ApiResponse<List<string>>
                {
                    Success = true,
                    Data = statuses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<string>>
                {
                    Success = false,
                    Error = "An error occurred while retrieving session statuses"
                });
            }
        }
    }

    public class CreateSessionRequest
    {
        public string SkillId { get; set; } = string.Empty;
        public string InstructorId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public int Duration { get; set; } = 0;
        public decimal Price { get; set; } = 0.00m;
        public string? Notes { get; set; }
    }

    public class UpdateSessionRequest
    {
        public string? Status { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public int? Duration { get; set; }
        public decimal? Price { get; set; }
        public string? MeetingUrl { get; set; }
        public string? Notes { get; set; }
        public decimal? Rating { get; set; }
        public string? Review { get; set; }
    }
} 