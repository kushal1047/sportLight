using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportLight.API.Models
{
    public class Skill
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Sport { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Difficulty { get; set; } = "beginner"; // beginner, intermediate, advanced, expert
        
        [Required]
        [StringLength(450)]
        public string InstructorId { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? VideoUrl { get; set; }
        
        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }
        
        [Required]
        public int Duration { get; set; } = 0; // in minutes
        
        [Required]
        public decimal Rating { get; set; } = 0.0m;
        
        [Required]
        public int ReviewCount { get; set; } = 0;
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; } = 0.00m;
        
        [Required]
        public bool IsPublished { get; set; } = false;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("InstructorId")]
        public virtual User Instructor { get; set; } = null!;
        public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
} 