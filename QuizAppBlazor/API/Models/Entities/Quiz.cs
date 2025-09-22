using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAppBlazor.API.Models.Entities
{
    /// <summary>
    /// Represents a quiz session taken by a user
    /// </summary>
    public class Quiz
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)] // Standard ASP.NET Identity user ID length
        public string UserId { get; set; } = string.Empty;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public int TotalQuestions { get; set; } = 0;

        public int CorrectAnswers { get; set; } = 0;

        public QuizStatus Status { get; set; } = QuizStatus.InProgress;

        public double? ScorePercentage { get; set; }

        public TimeSpan? TimeTaken { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
        
        public virtual ICollection<QuizQuestionAnswer> Answers { get; set; } = new List<QuizQuestionAnswer>();

        /// <summary>
        /// Calculate score percentage based on correct answers
        /// </summary>
        public void CalculateScore()
        {
            if (TotalQuestions > 0)
            {
                ScorePercentage = (double)CorrectAnswers / TotalQuestions * 100;
            }
        }

        /// <summary>
        /// Mark quiz as completed
        /// </summary>
        public void Complete()
        {
            CompletedAt = DateTime.UtcNow;
            Status = QuizStatus.Completed;
            TimeTaken = CompletedAt - StartedAt;
            CalculateScore();
        }
    }

    /// <summary>
    /// Enum for quiz status
    /// </summary>
    public enum QuizStatus
    {
        InProgress = 1,
        Completed = 2,
        Abandoned = 3,
        TimedOut = 4
    }
}
