using QuizAppBlazor.API.DTOs;
using QuizAppBlazor.API.HttpResponse;

namespace QuizAppBlazor.API.Services
{
    public interface IScoreService
    {
        Task<ResponseBaseHttp<IEnumerable<GetScoreByAuthorDTO>>> GetScoresByUserAsync(string userId, string userNickname = "", int skip = 0, int take = 10, bool isAdmin = false);
        Task<ResponseBaseHttp<string>> CreateScoreAsync(string userId, UserScoreDTO userScore);
    }
}
