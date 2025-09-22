using QuizAppBlazor.API.DTOs;
using QuizAppBlazor.API.HttpResponse;

namespace QuizAppBlazor.API.Services
{
    public interface IQuestionService
    {
        Task<ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>> GetQuestionsAsync(string question = "", int skip = 0, int take = 10);
        Task<ResponseBaseHttp<GetQuestionsDTO>> GetQuestionByIdAsync(int id);
        Task<IEnumerable<GetQuestionsDTO>> GetRandomQuestionsAsync(int count = 15);
        Task<ResponseBaseHttp<string>> CreateQuestionAsync(CreateQuestionDTO questionDto);
        Task<ResponseBaseHttp<string>> UpdateQuestionAsync(int id, CreateQuestionDTO questionDto);
        Task<ResponseBaseHttp<string>> DeleteQuestionAsync(int id);
    }
}
