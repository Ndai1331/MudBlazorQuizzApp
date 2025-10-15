using System.ComponentModel.DataAnnotations;

namespace QuizAppBlazor.Client.DTOs
{
    public class CreateQuestionDTO
    {
        [Required(ErrorMessage = "Câu hỏi không được để trống")]
        [StringLength(500, ErrorMessage = "Câu hỏi không được vượt quá 500 ký tự")]
        [MinLength(10, ErrorMessage = "Câu hỏi phải có ít nhất 10 ký tự")]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "Đáp án đúng không được để trống")]
        [StringLength(200, ErrorMessage = "Đáp án không được vượt quá 200 ký tự")]
        [MinLength(1, ErrorMessage = "Đáp án phải có ít nhất 1 ký tự")]
        public string CorrectAnswer { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Đáp án không được vượt quá 200 ký tự")]
        public string Alternativ2 { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Đáp án không được vượt quá 200 ký tự")]
        public string Alternativ3 { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Đáp án không được vượt quá 200 ký tự")]
        public string Alternativ4 { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Text input không được vượt quá 200 ký tự")]
        public string UserTextInput { get; set; } = string.Empty;

        public bool IsTextInput { get; set; } = false;

        [StringLength(500, ErrorMessage = "URL media không được vượt quá 500 ký tự")]
        [Url(ErrorMessage = "URL media không hợp lệ")]
        public string ImageVideo { get; set; } = string.Empty;

        public bool IsImage { get; set; } = false;

        public bool IsVideo { get; set; } = false;

        public bool IsYoutubeVideo { get; set; } = false;

        public bool HasTimeLimit { get; set; } = true;

        [Range(10, 300, ErrorMessage = "Thời gian phải từ 10 đến 300 giây")]
        public int TimeLimit { get; set; } = 60;
        public QuestionTypeEnum Type { get; set; } = QuestionTypeEnum.QD;

        /// <summary>
        /// Validates that at least one alternative answer is provided for multiple choice questions
        /// </summary>
        public bool IsValid()
        {
            if (!IsTextInput)
            {
                // For multiple choice, we need at least one alternative answer
                return !string.IsNullOrWhiteSpace(Alternativ2) || 
                       !string.IsNullOrWhiteSpace(Alternativ3) || 
                       !string.IsNullOrWhiteSpace(Alternativ4);
            }
            return true;
        }
    }
}
