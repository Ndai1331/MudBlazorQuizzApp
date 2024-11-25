using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAppBlazor.API.Models
{
    public class ScoreModel
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public int CorrectAnswers { get; set; }
        public string Questions { get; set; }
        public string Answers { get; set; }
        public string Corrects { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
