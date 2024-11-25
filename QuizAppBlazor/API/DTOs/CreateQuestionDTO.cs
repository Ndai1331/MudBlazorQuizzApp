using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAppBlazor.API.DTOs
{
    public class CreateQuestionDTO
    {

        [Required]
        public string Question { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        public string Alternativ2 { get; set; } = string.Empty;

        public string Alternativ3 { get; set; } = string.Empty;

        public string Alternativ4 { get; set; } = string.Empty;

        public string UserTextInput { get; set; } = string.Empty;

        public bool IsTextInput { get; set; }

        public string ImageVideo { get; set; } = string.Empty;

        public bool IsImage { get; set; }

        public bool IsVideo { get; set; } //Probably will be most compatible with MP4 format

        public bool IsYoutubeVideo { get; set; }

        public bool HasTimeLimit { get; set; } = true;

        public int TimeLimit { get; set; } = 20;
    }
}
