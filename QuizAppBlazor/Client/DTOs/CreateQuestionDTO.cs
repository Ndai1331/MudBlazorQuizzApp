using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAppBlazor.Client.DTOs
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

        public bool IsTextInput { get; set; } = false;

        public string ImageVideo { get; set; } = string.Empty;

        public bool IsImage { get; set; } = false;

        public bool IsVideo { get; set; } = false;  //Probably will be most compatible with MP4 format

        public bool IsYoutubeVideo { get; set; } = false;

        public bool HasTimeLimit { get; set; } = true;

        public int TimeLimit { get; set; } = 60;
    }
}
