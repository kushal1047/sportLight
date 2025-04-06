using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportLight.API.Models
{
    public class Session
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(450)]
        public string SkillId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(450)]
        public string InstructorId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(450)]
        public string StudentId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "scheduled"; // scheduled, in-progress, completed, cancelled
        
        [Required]
        public DateTime ScheduledAt { get; set; }
        
        [Required]
        public int Duration { get; set; } = 0; // in minutes
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; } = 0.00m;
        
        [StringLength(500)]
        public string? MeetingUrl { get; set; }
        
        [StringLength(2000)]
        public string? Notes { get; set; }
        
        public decimal? Rating { get; set; }
        
        [StringLength(1000)]
        public string? Review { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("SkillId")]
        public virtual Skill Skill { get; set; } = null!;
        
        [ForeignKey("InstructorId")]
        public virtual User Instructor { get; set; } = null!;
        
        [ForeignKey("StudentId")]
        public virtual User Student { get; set; } = null!;
    }
} 