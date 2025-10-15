using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAppBlazor.API.Models.Entities
{
    /// <summary>
    /// Represents a quiz question with improved structure
    /// </summary>
    public class Question
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string CorrectAnswer { get; set; } = string.Empty;

        public bool IsTextInput { get; set; } = false;

        public bool HasTimeLimit { get; set; } = true;

        [Range(10, 300)]
        public int TimeLimit { get; set; } = 60;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public QuestionTypeEnum? Type { get; set; } = QuestionTypeEnum.QD;


        // Navigation properties
        public virtual ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
        public virtual ICollection<QuestionMedia> Media { get; set; } = new List<QuestionMedia>();
        public virtual ICollection<QuizQuestionAnswer> QuizAnswers { get; set; } = new List<QuizQuestionAnswer>();
    }
}
