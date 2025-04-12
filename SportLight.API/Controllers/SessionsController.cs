using Microsoft.AspNetCore.Mvc;
using SportLight.API.Data;
using SportLight.API.DTOs;
using SportLight.API.Models;
using Microsoft.EntityFrameworkCore;

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
            [FromQuery] string? status = null,
            [FromQuery] string? instructorId = null,
            [FromQuery] string? studentId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Sessions
                    .Include(s => s.Skill)
                    .Include(s => s.Instructor)
                    .Include(s => s.Student)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(s => s.Status == status);

                if (!string.IsNullOrEmpty(instructorId))
                    query = query.Where(s => s.InstructorId == instructorId);

                if (!string.IsNullOrEmpty(studentId))
                    query = query.Where(s => s.StudentId == studentId);

                var totalCount = await query.CountAsync();
                var sessions = await query
                    .OrderByDescending(s => s.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new ApiResponse<List<Session>>
                {
                    Success = true,
                    Data = sessions,
                    Message = $"Retrieved {sessions.Count} sessions"
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
                    Error = "An error occurred while retrieving the session"
                });
            }
        }

        [HttpGet("statuses")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetSessionStatuses()
        {
            try
            {
                var statuses = await _context.Sessions
                    .Select(s => s.Status)
                    .Distinct()
                    .ToListAsync();

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

        [HttpGet("upcoming")]
        public async Task<ActionResult<ApiResponse<List<Session>>>> GetUpcomingSessions(
            [FromQuery] string? userId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Sessions
                    .Include(s => s.Skill)
                    .Include(s => s.Instructor)
                    .Include(s => s.Student)
                    .Where(s => s.ScheduledAt > DateTime.UtcNow && s.Status == "scheduled")
                    .AsQueryable();

                if (!string.IsNullOrEmpty(userId))
                    query = query.Where(s => s.InstructorId == userId || s.StudentId == userId);

                var totalCount = await query.CountAsync();
                var sessions = await query
                    .OrderBy(s => s.ScheduledAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new ApiResponse<List<Session>>
                {
                    Success = true,
                    Data = sessions,
                    Message = $"Retrieved {sessions.Count} upcoming sessions"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<Session>>
                {
                    Success = false,
                    Error = "An error occurred while retrieving upcoming sessions"
                });
            }
        }
    }
} 