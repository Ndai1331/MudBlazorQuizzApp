using Microsoft.EntityFrameworkCore;
using QuizAppBlazor.API.Data;
using QuizAppBlazor.API.DTOs;
using QuizAppBlazor.API.HttpResponse;
using QuizAppBlazor.API.Models;

namespace QuizAppBlazor.API.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(ApplicationDbContext context, ILogger<QuestionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>> GetQuestionsAsync(string question = "", int skip = 0, int take = 10)
        {
            try
            {
                var query = _context.Questions
                    .Where(q => string.IsNullOrEmpty(question) || 
                               q.Question.Trim().ToLower().Contains(question.Trim().ToLower()))
                    .OrderByDescending(q => q.Id)
                    .Select(q => new GetQuestionsDTO
                    {
                        Id = q.Id,
                        Question = q.Question,
                        CorrectAnswer = q.CorrectAnswer,
                        Alternativ2 = q.Alternativ2,
                        Alternativ3 = q.Alternativ3,
                        Alternativ4 = q.Alternativ4,
                        IsTextInput = q.IsTextInput,
                        ImageVideo = q.ImageVideo,
                        IsImage = q.IsImage,
                        IsVideo = q.IsVideo,
                        IsYoutubeVideo = q.IsYoutubeVideo,
                        HasTimeLimit = q.HasTimeLimit,
                        TimeLimit = q.TimeLimit,
                        Type = q.Type
                    });

                int totalRecords = await query.CountAsync();
                int totalPages = (int)Math.Ceiling((double)totalRecords / take);

                var result = await query.Skip(skip).Take(take).ToListAsync();

                return new ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>
                {
                    Result = result,
                    TotalPages = totalPages,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting questions with search: {Question}", question);
                throw;
            }
        }

        public async Task<ResponseBaseHttp<GetQuestionsDTO>> GetQuestionByIdAsync(int id)
        {
            try
            {
                var result = await _context.Questions
                    .Where(q => q.Id == id)
                    .Select(q => new GetQuestionsDTO
                    {
                        Id = q.Id,
                        Question = q.Question,
                        CorrectAnswer = q.CorrectAnswer,
                        Alternativ2 = q.Alternativ2,
                        Alternativ3 = q.Alternativ3,
                        Alternativ4 = q.Alternativ4,
                        IsTextInput = q.IsTextInput,
                        ImageVideo = q.ImageVideo ?? string.Empty,
                        IsImage = q.IsImage,
                        IsVideo = q.IsVideo,
                        IsYoutubeVideo = q.IsYoutubeVideo,
                        HasTimeLimit = q.HasTimeLimit,
                        TimeLimit = q.TimeLimit,
                        Type = q.Type
                    })
                    .FirstOrDefaultAsync();

                return new ResponseBaseHttp<GetQuestionsDTO>
                {
                    Result = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting question by id: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<GetQuestionsDTO>> GetRandomQuestionsAsync(int count = 15)
        {
            try
            {
                var result = await _context.Questions
                    .OrderBy(x => Guid.NewGuid())
                    .Take(count)
                    .Select(x => new GetQuestionsDTO
                    {
                        Id = x.Id,
                        Question = x.Question,
                        CorrectAnswer = x.CorrectAnswer,
                        Alternativ2 = x.Alternativ2,
                        Alternativ3 = x.Alternativ3,
                        Alternativ4 = x.Alternativ4,
                        IsTextInput = x.IsTextInput,
                        ImageVideo = x.ImageVideo,
                        IsImage = x.IsImage,
                        IsVideo = x.IsVideo,
                        IsYoutubeVideo = x.IsYoutubeVideo,
                        HasTimeLimit = x.HasTimeLimit,
                        TimeLimit = x.TimeLimit,
                        Type = x.Type
                    })
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} random questions", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting random questions");
                throw;
            }
        }

        public async Task<ResponseBaseHttp<string>> CreateQuestionAsync(CreateQuestionDTO questionDto)
        {
            try
            {
                // Additional custom validation
                if (!questionDto.IsValid())
                {
                    return new ResponseBaseHttp<string> 
                    { 
                        Result = "Câu hỏi trắc nghiệm phải có ít nhất một đáp án thay thế" 
                    };
                }

                var questionModel = new QuestionModel
                {
                    Question = questionDto.Question,
                    CorrectAnswer = questionDto.CorrectAnswer,
                    Alternativ2 = questionDto.Alternativ2,
                    Alternativ3 = questionDto.Alternativ3,
                    Alternativ4 = questionDto.Alternativ4,
                    IsTextInput = questionDto.IsTextInput,
                    ImageVideo = questionDto.ImageVideo,
                    IsImage = questionDto.IsImage,
                    IsVideo = questionDto.IsVideo,
                    IsYoutubeVideo = questionDto.IsYoutubeVideo,
                    HasTimeLimit = questionDto.HasTimeLimit,
                    TimeLimit = questionDto.TimeLimit,
                    Type = questionDto.Type
                };

                _context.Questions.Add(questionModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created new question with id: {Id}", questionModel.Id);
                return new ResponseBaseHttp<string> { Result = string.Empty };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                return new ResponseBaseHttp<string> { Result = ex.Message };
            }
        }

        public async Task<ResponseBaseHttp<string>> UpdateQuestionAsync(int id, CreateQuestionDTO questionDto)
        {
            try
            {
                // Additional custom validation
                if (!questionDto.IsValid())
                {
                    return new ResponseBaseHttp<string> 
                    { 
                        Result = "Câu hỏi trắc nghiệm phải có ít nhất một đáp án thay thế" 
                    };
                }

                var questionModel = await _context.Questions.FindAsync(id);
                if (questionModel == null)
                {
                    return new ResponseBaseHttp<string> { Result = "Không tìm thấy câu hỏi" };
                }

                questionModel.Question = questionDto.Question;
                questionModel.CorrectAnswer = questionDto.CorrectAnswer;
                questionModel.Alternativ2 = questionDto.Alternativ2;
                questionModel.Alternativ3 = questionDto.Alternativ3;
                questionModel.Alternativ4 = questionDto.Alternativ4;
                questionModel.IsTextInput = questionDto.IsTextInput;
                questionModel.ImageVideo = questionDto.ImageVideo;
                questionModel.IsImage = questionDto.IsImage;
                questionModel.IsVideo = questionDto.IsVideo;
                questionModel.IsYoutubeVideo = questionDto.IsYoutubeVideo;
                questionModel.HasTimeLimit = questionDto.HasTimeLimit;
                questionModel.TimeLimit = questionDto.TimeLimit;
                questionModel.Type = questionDto.Type;
                _context.Questions.Update(questionModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated question with id: {Id}", id);
                return new ResponseBaseHttp<string> { Result = string.Empty };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question with id: {Id}", id);
                return new ResponseBaseHttp<string> { Result = ex.Message };
            }
        }

        public async Task<ResponseBaseHttp<string>> DeleteQuestionAsync(int id)
        {
            try
            {
                var questionModel = await _context.Questions.FindAsync(id);
                if (questionModel == null)
                {
                    return new ResponseBaseHttp<string> { Result = "Không tìm thấy câu hỏi" };
                }

                _context.Questions.Remove(questionModel);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    _logger.LogInformation("Deleted question with id: {Id}", id);
                    return new ResponseBaseHttp<string> { Result = string.Empty };
                }

                return new ResponseBaseHttp<string> { Result = "Không thể xóa câu hỏi." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question with id: {Id}", id);
                return new ResponseBaseHttp<string> { Result = ex.Message };
            }
        }
    }
}
