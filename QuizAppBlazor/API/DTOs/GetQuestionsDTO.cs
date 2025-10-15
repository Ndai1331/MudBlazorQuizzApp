using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAppBlazor.API.DTOs
{
    public class GetQuestionsDTO
    {
        public int Id { get; set; }

        public string Question { get; set; }

        public string CorrectAnswer { get; set; }

        public string? Alternativ2 { get; set; }

        public string? Alternativ3 { get; set; }

        public string? Alternativ4 { get; set; }

        public bool? IsTextInput { get; set; }

        public string? ImageVideo { get; set; }

        public bool? IsImage { get; set; }

        public bool? IsVideo { get; set; }

        public bool? IsYoutubeVideo { get; set; }

        public bool? HasTimeLimit { get; set; }

        public int? TimeLimit { get; set; }
        public QuestionTypeEnum Type { get; set; } = QuestionTypeEnum.QD;

    }
}
