namespace QuizAppBlazor.Client.DTOs
{
    public class UserScoreDTO
    {
        public List<int> QuestionIndex { get; set; } = new List<int>();
        public List<string> Questions { get; set; } = new List<string>();
        public List<string> Answers { get; set; } = new List<string>();
        public List<string> Corrects { get; set; } = new List<string>();
        public int CorrectAnswers { get; set; }
    }
}
