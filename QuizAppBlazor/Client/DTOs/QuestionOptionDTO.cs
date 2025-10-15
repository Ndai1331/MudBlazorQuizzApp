using System.ComponentModel.DataAnnotations;

namespace QuizAppBlazor.Client.DTOs
{
    public class QuestionOptionDTO
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        
        [Required(ErrorMessage = "Nội dung lựa chọn không được để trống")]
        [StringLength(200, ErrorMessage = "Nội dung lựa chọn không được vượt quá 200 ký tự")]
        public string Content { get; set; } = string.Empty;
        
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class QuestionMediaDTO
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        
        [Required(ErrorMessage = "URL media không được để trống")]
        [StringLength(500, ErrorMessage = "URL media không được vượt quá 500 ký tự")]
        [Url(ErrorMessage = "URL media không hợp lệ")]
        public string Url { get; set; } = string.Empty;
        
        public MediaType Type { get; set; }
        
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự")]
        public string? Title { get; set; }
        
        [StringLength(200, ErrorMessage = "Mô tả không được vượt quá 200 ký tự")]
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }

    public enum MediaType
    {
        Image = 1,
        Video = 2,
        YouTube = 3,
        Audio = 4
    }

    /// <summary>
    /// Enhanced Question DTO with new structure
    /// </summary>
    public class QuestionV2DTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Câu hỏi không được để trống")]
        [StringLength(500, ErrorMessage = "Câu hỏi không được vượt quá 500 ký tự")]
        public string Content { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Đáp án đúng không được để trống")]
        [StringLength(200, ErrorMessage = "Đáp án không được vượt quá 200 ký tự")]
        public string CorrectAnswer { get; set; } = string.Empty;
        
        public bool IsTextInput { get; set; } = false;
        public bool HasTimeLimit { get; set; } = true;
        
        [Range(10, 300, ErrorMessage = "Thời gian phải từ 10 đến 300 giây")]
        public int TimeLimit { get; set; } = 60;
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public QuestionTypeEnum Type { get; set; } = QuestionTypeEnum.QD;
        
        // Navigation properties
        public List<QuestionOptionDTO> Options { get; set; } = new List<QuestionOptionDTO>();
        public List<QuestionMediaDTO> Media { get; set; } = new List<QuestionMediaDTO>();
    }
}
