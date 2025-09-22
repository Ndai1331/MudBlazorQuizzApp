namespace QuizAppBlazor.Client.DTOs
{
    public class QuizDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public QuizStatus Status { get; set; }
        public double? ScorePercentage { get; set; }
        public TimeSpan? TimeTaken { get; set; }
        public List<QuizQuestionAnswerDTO> Answers { get; set; } = new List<QuizQuestionAnswerDTO>();
    }

    public class QuizQuestionAnswerDTO
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public string UserAnswer { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public DateTime AnsweredAt { get; set; }
        public TimeSpan? TimeSpent { get; set; }
        public int QuestionOrder { get; set; }
    }

    public class CreateQuizDTO
    {
        public string UserId { get; set; } = string.Empty;
        public int TotalQuestions { get; set; } = 15;
    }

    public class SubmitQuizAnswerDTO
    {
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public string UserAnswer { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public TimeSpan? TimeSpent { get; set; }
        public int QuestionOrder { get; set; }
    }

    public enum QuizStatus
    {
        InProgress = 1,
        Completed = 2,
        Abandoned = 3,
        TimedOut = 4
    }
}
