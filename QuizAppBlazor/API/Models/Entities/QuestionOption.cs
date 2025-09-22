using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAppBlazor.API.Models.Entities
{
    /// <summary>
    /// Represents an option/answer choice for a multiple choice question
    /// </summary>
    public class QuestionOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Content { get; set; } = string.Empty;

        public bool IsCorrect { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey(nameof(QuestionId))]
        public virtual Question Question { get; set; } = null!;
    }
}
