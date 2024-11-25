using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAppBlazor.API.DTOs
{
    public class UserScoreDTO
    {
        [Required, MinLength(1)]
        public List<string> Questions { get; set; } = new List<string>();
        [Required, MinLength(1)]
        public List<string> Answers { get; set; } = new List<string>();
        [Required, MinLength(1)]
        public List<string> Corrects { get; set; } = new List<string>();
        [Required]
        public int CorrectAnswers { get; set; }
    }
}
