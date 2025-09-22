using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAppBlazor.API.Models.Entities
{
    /// <summary>
    /// Represents media content associated with a question
    /// </summary>
    public class QuestionMedia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(500)]
        [Url]
        public string Url { get; set; } = string.Empty;

        [Required]
        public MediaType Type { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey(nameof(QuestionId))]
        public virtual Question Question { get; set; } = null!;
    }

    /// <summary>
    /// Enum for media types
    /// </summary>
    public enum MediaType
    {
        Image = 1,
        Video = 2,
        YouTube = 3,
        Audio = 4
    }
}
