using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace QuizAppBlazor.API.Models
{
    //This model could be devided into at least two models, Question and Media
    public class QuestionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        [AllowNull]
        public string Alternativ2 { get; set; }

        [AllowNull]
        public string Alternativ3 { get; set; }

        [AllowNull]
        public string Alternativ4 { get; set; }

        // If this is false then it is options
        [AllowNull]
        public bool? IsTextInput { get; set; }

        [AllowNull]
        public string? ImageVideo { get; set; }

        [AllowNull]
        public bool? IsImage { get; set; }

        [AllowNull]
        public bool? IsVideo { get; set; } //Probably will be most compatible with MP4 format

        [AllowNull]
        public bool? IsYoutubeVideo { get; set; }

        [AllowNull]
        public bool? HasTimeLimit { get; set; }

        [AllowNull]
        public int? TimeLimit { get; set; } = 60;


        public QuestionTypeEnum Type { get; set; } = QuestionTypeEnum.QD;

    }
}
