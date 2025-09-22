using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAppBlazor.API.Models.Entities
{
    /// <summary>
    /// Represents an answer given by a user for a specific question in a quiz
    /// </summary>
    public class QuizQuestionAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(500)]
        public string UserAnswer { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string CorrectAnswer { get; set; } = string.Empty;

        public bool IsCorrect { get; set; } = false;

        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        public TimeSpan? TimeSpent { get; set; }

        public int QuestionOrder { get; set; } = 0;

        // Navigation properties
        [ForeignKey(nameof(QuizId))]
        public virtual Quiz Quiz { get; set; } = null!;

        [ForeignKey(nameof(QuestionId))]
        public virtual Question Question { get; set; } = null!;

        /// <summary>
        /// Check if the user's answer is correct
        /// </summary>
        public void ValidateAnswer()
        {
            IsCorrect = string.Equals(
                UserAnswer?.Trim(), 
                CorrectAnswer?.Trim(), 
                StringComparison.OrdinalIgnoreCase
            );
        }
    }
}
