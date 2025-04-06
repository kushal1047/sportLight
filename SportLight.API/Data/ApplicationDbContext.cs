using Microsoft.EntityFrameworkCore;
using SportLight.API.Models;

namespace SportLight.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Configure Skill entity
            modelBuilder.Entity<Skill>(entity =>
            {
                entity.HasIndex(e => e.InstructorId);
                entity.HasIndex(e => e.Sport);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Difficulty);
                entity.HasIndex(e => e.IsPublished);
            });

            // Configure Session entity
            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasIndex(e => e.SkillId);
                entity.HasIndex(e => e.InstructorId);
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ScheduledAt);
            });

            // Configure Review entity
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasIndex(e => e.SkillId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Rating);
            });

            // Configure relationships
            modelBuilder.Entity<Skill>()
                .HasOne(s => s.Instructor)
                .WithMany(u => u.Skills)
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Skill)
                .WithMany(sk => sk.Sessions)
                .HasForeignKey(s => s.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Instructor)
                .WithMany(u => u.InstructorSessions)
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Student)
                .WithMany(u => u.StudentSessions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Skill)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 