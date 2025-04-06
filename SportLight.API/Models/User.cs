using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportLight.API.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Avatar { get; set; }
        
        [StringLength(1000)]
        public string? Bio { get; set; }
        
        [Required]
        public string SportsInterests { get; set; } = "[]"; // JSON array of sports
        
        [Required]
        [StringLength(20)]
        public string SkillLevel { get; set; } = "beginner"; // beginner, intermediate, advanced, expert
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public virtual ICollection<Session> InstructorSessions { get; set; } = new List<Session>();
        public virtual ICollection<Session> StudentSessions { get; set; } = new List<Session>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
} 