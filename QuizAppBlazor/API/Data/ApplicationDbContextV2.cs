using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizAppBlazor.API.Models;
using QuizAppBlazor.API.Models.Entities;

namespace QuizAppBlazor.API.Data
{
    /// <summary>
    /// Improved database context with better entity relationships
    /// </summary>
    public class ApplicationDbContextV2 : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContextV2(DbContextOptions<ApplicationDbContextV2> options)
            : base(options) { }

        // New improved entities
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<QuestionMedia> QuestionMedia { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestionAnswer> QuizQuestionAnswers { get; set; }

        // Keep old entities for backward compatibility during migration
        public DbSet<QuestionModel> OldQuestions { get; set; }
        public DbSet<ScoreModel> OldScores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Question entity
            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Questions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CorrectAnswer).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedAt);
            });

            // Configure QuestionOption entity
            modelBuilder.Entity<QuestionOption>(entity =>
            {
                entity.ToTable("QuestionOptions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(200);
                
                entity.HasOne(e => e.Question)
                      .WithMany(e => e.Options)
                      .HasForeignKey(e => e.QuestionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.QuestionId, e.DisplayOrder });
            });

            // Configure QuestionMedia entity
            modelBuilder.Entity<QuestionMedia>(entity =>
            {
                entity.ToTable("QuestionMedia");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Title).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                
                entity.HasOne(e => e.Question)
                      .WithMany(e => e.Media)
                      .HasForeignKey(e => e.QuestionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Quiz entity
            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.ToTable("Quizzes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.ScorePercentage).HasPrecision(5, 2);
                
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.StartedAt);
                entity.HasIndex(e => e.Status);
            });

            // Configure QuizQuestionAnswer entity
            modelBuilder.Entity<QuizQuestionAnswer>(entity =>
            {
                entity.ToTable("QuizQuestionAnswers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserAnswer).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CorrectAnswer).IsRequired().HasMaxLength(500);
                
                entity.HasOne(e => e.Quiz)
                      .WithMany(e => e.Answers)
                      .HasForeignKey(e => e.QuizId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Question)
                      .WithMany(e => e.QuizAnswers)
                      .HasForeignKey(e => e.QuestionId)
                      .OnDelete(DeleteBehavior.Restrict); // Don't delete questions if they have answers

                entity.HasIndex(e => new { e.QuizId, e.QuestionOrder });
                entity.HasIndex(e => e.IsCorrect);
            });

            // Configure old entities for backward compatibility
            modelBuilder.Entity<QuestionModel>(entity =>
            {
                entity.ToTable("OldQuestions");
            });

            modelBuilder.Entity<ScoreModel>(entity =>
            {
                entity.ToTable("OldScores");
            });

            // Add some constraints and indexes for performance
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(e => e.Nickname).IsUnique();
                entity.Property(e => e.Nickname).HasMaxLength(50);
                entity.Property(e => e.Role).HasMaxLength(20);
            });
        }

        /// <summary>
        /// Override SaveChanges to automatically set UpdatedAt timestamps
        /// </summary>
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        /// <summary>
        /// Override SaveChangesAsync to automatically set UpdatedAt timestamps
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Question && e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is Question question)
                {
                    question.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
