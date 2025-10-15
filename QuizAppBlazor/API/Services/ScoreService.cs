using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using QuizAppBlazor.API.Data;
using QuizAppBlazor.API.DTOs;
using QuizAppBlazor.API.HttpResponse;
using QuizAppBlazor.API.Models;

namespace QuizAppBlazor.API.Services
{
    public class ScoreService : IScoreService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ScoreService> _logger;

        public ScoreService(ApplicationDbContext context, ILogger<ScoreService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseBaseHttp<IEnumerable<GetScoreByAuthorDTO>>> GetScoresByUserAsync(
            string userId, 
            string userNickname = "", 
            int skip = 0, 
            int take = 10, 
            bool isAdmin = false)
        {
            try
            {
                var query = from s in _context.Score
                           join u in _context.Users on s.UserId equals u.Id
                           where (string.IsNullOrEmpty(userNickname) || 
                                 u.Nickname.Trim().ToLower().Contains(userNickname.Trim().ToLower()))
                                 //&& (!isAdmin || s.UserId == userId)
                           orderby s.Date descending
                           select new GetScoreByAuthorDTO
                           {
                               Nickname = u.Nickname,
                               Points = s.CorrectAnswers,
                               Questions = s.Questions,
                               Answers = s.Answers,
                               Corrects = s.Corrects,
                               Date = s.Date
                           };

                int totalRecords = await query.CountAsync();
                int totalPages = (int)Math.Ceiling((double)totalRecords / take);

                var result = await query.Skip(skip).Take(take).ToListAsync();

                return new ResponseBaseHttp<IEnumerable<GetScoreByAuthorDTO>>
                {
                    Result = result,
                    TotalPages = totalPages,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting scores for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<ResponseBaseHttp<string>> CreateScoreAsync(string userId, UserScoreDTO userScore)
        {
            try
            {
                // Check for recent duplicate score (within last 5 minutes)
                var recentScore = await _context.Score
                    .Where(s => s.UserId == userId && 
                               s.CorrectAnswers == userScore.CorrectAnswers &&
                               s.Date > DateTime.UtcNow.AddMinutes(-5))
                    .FirstOrDefaultAsync();

                if (recentScore != null)
                {
                    _logger.LogWarning("Duplicate score attempt detected for user: {UserId}", userId);
                    return new ResponseBaseHttp<string> { Result = "Score already saved recently" };
                }

                var scoreModel = new ScoreModel
                {
                    UserId = userId,
                    Questions = string.Join("|", userScore.Questions),
                    Answers = string.Join("|", userScore.Answers),
                    Corrects = string.Join("|", userScore.Corrects),
                    CorrectAnswers = userScore.CorrectAnswers,
                    Date = DateTime.UtcNow
                };

                _context.Score.Add(scoreModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created score for user: {UserId} with {CorrectAnswers} correct answers", 
                    userId, userScore.CorrectAnswers);

                return new ResponseBaseHttp<string> { Result = string.Empty };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating score for user: {UserId}", userId);
                return new ResponseBaseHttp<string> { Result = ex.Message };
            }
        }
    }
}
