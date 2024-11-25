using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAppBlazor.Client.DTOs
{
    public class GetQuestionsDTO
    {
        public int Id { get; set; }

        public string QuestionId { get; set; } = "";

        public string Question { get; set; } = "";

        public string CorrectAnswer { get; set; } = "";

        public string Alternativ2 { get; set; } = "";

        public string Alternativ3 { get; set; } = "";

        public string Alternativ4 { get; set; } = "";

        public bool IsTextInput { get; set; } = false;

        public string ImageVideo { get; set; } = "";

        public bool IsImage { get; set; } = false;

        public bool IsVideo { get; set; } = false;

        public bool IsYoutubeVideo { get; set; } = false;

        public bool HasTimeLimit { get; set; } = true;

        public int TimeLimit { get; set; } = 60;
    }
}
