using Microsoft.AspNetCore.Mvc;

namespace SportLight.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Status = "Healthy",
                Message = "SportLight API is running",
                Timestamp = DateTime.UtcNow
            });
        }
    }
} 