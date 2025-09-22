using QuizAppBlazor.Client.DTOs;
using QuizAppBlazor.Client.HttpResponse;

namespace QuizAppBlazor.Client.Services
{
    /// <summary>
    /// Service interface for API communication with improved error handling
    /// </summary>
    public interface IApiService
    {
        // Question endpoints
        Task<ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>> GetQuestionsAsync(string search = "", int skip = 0, int take = 10);
        Task<ResponseBaseHttp<GetQuestionsDTO>> GetQuestionByIdAsync(int id);
        Task<IEnumerable<GetQuestionsDTO>> GetRandomQuestionsAsync();
        Task<ResponseBaseHttp<string>> CreateQuestionAsync(CreateQuestionDTO question);
        Task<ResponseBaseHttp<string>> UpdateQuestionAsync(int id, CreateQuestionDTO question);
        Task<ResponseBaseHttp<string>> DeleteQuestionAsync(int id);

        // User endpoints
        Task<UserAuthDto> LoginAsync(UserLoginDto loginDto);
        Task<ResponseBaseHttp<string>> RegisterAsync(UserLoginDto registerDto);
        Task<UserAuthDto> GetUserByIdAsync(string id);
        Task<ResponseBaseHttp<string>> UpdateUserAsync(UserLoginDto userDto);

        // Score endpoints
        Task<ResponseBaseHttp<IEnumerable<GetScoreByAuthorDTO>>> GetScoresAsync(string userNickname = "", int skip = 0, int take = 10, bool isAdmin = false);
        Task<ResponseBaseHttp<string>> SubmitScoreAsync(UserScoreDTO score);

        // Quiz endpoints (new)
        Task<QuizDTO> StartQuizAsync(CreateQuizDTO createQuizDto);
        Task<ResponseBaseHttp<string>> SubmitQuizAnswerAsync(SubmitQuizAnswerDTO answerDto);
        Task<QuizDTO> CompleteQuizAsync(int quizId);
        Task<ResponseBaseHttp<IEnumerable<QuizDTO>>> GetUserQuizzesAsync(string userId, int skip = 0, int take = 10);
    }
}
