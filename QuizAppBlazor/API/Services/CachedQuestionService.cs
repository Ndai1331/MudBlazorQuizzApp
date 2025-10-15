using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using QuizAppBlazor.API.DTOs;
using QuizAppBlazor.API.HttpResponse;

namespace QuizAppBlazor.API.Services
{
    public class CachedQuestionService : IQuestionService
    {
        private readonly IQuestionService _questionService;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<CachedQuestionService> _logger;

        private readonly MemoryCacheEntryOptions _defaultCacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
            SlidingExpiration = TimeSpan.FromMinutes(5),
            Priority = CacheItemPriority.Normal
        };

        private readonly DistributedCacheEntryOptions _distributedCacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            SlidingExpiration = TimeSpan.FromMinutes(15)
        };

        public CachedQuestionService(
            IQuestionService questionService, 
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ILogger<CachedQuestionService> logger)
        {
            _questionService = questionService;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>> GetQuestionsAsync(string question = "", int skip = 0, int take = 10)
        {
            // Don't cache paginated results with search as they change frequently
            return await _questionService.GetQuestionsAsync(question, skip, take);
        }

        public async Task<ResponseBaseHttp<GetQuestionsDTO>> GetQuestionByIdAsync(int id)
        {
            var cacheKey = $"question_{id}";
            
            // Try memory cache first
            if (_memoryCache.TryGetValue(cacheKey, out ResponseBaseHttp<GetQuestionsDTO> cachedQuestion))
            {
                _logger.LogDebug("Question {Id} found in memory cache", id);
                return cachedQuestion;
            }

            // Try distributed cache (skip if disabled)
            if (_distributedCache != null)
            {
                try
                {
                    var distributedCachedData = await _distributedCache.GetStringAsync(cacheKey);
                    if (!string.IsNullOrEmpty(distributedCachedData))
                    {
                        var cachedResult = JsonSerializer.Deserialize<ResponseBaseHttp<GetQuestionsDTO>>(distributedCachedData);
                        if (cachedResult != null)
                        {
                            // Store in memory cache for faster access
                            _memoryCache.Set(cacheKey, cachedResult, _defaultCacheOptions);
                            _logger.LogDebug("Question {Id} found in distributed cache", id);
                            return cachedResult;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to retrieve question {Id} from distributed cache", id);
                }
            }

            // Get from database
            var result = await _questionService.GetQuestionByIdAsync(id);
            
            if (result?.Result != null)
            {
                // Cache the result in memory
                _memoryCache.Set(cacheKey, result, _defaultCacheOptions);
                
                // Cache in distributed cache only if available
                if (_distributedCache != null)
                {
                    try
                    {
                        var serializedData = JsonSerializer.Serialize(result);
                        await _distributedCache.SetStringAsync(cacheKey, serializedData, _distributedCacheOptions);
                        _logger.LogDebug("Question {Id} cached successfully", id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to cache question {Id} in distributed cache", id);
                    }
                }
            }

            return result;
        }

        public async Task<IEnumerable<GetQuestionsDTO>> GetRandomQuestionsAsync(int count = 15)
        {
            var cacheKey = $"random_questions_{count}";
            
            // Try memory cache first (shorter cache time for random questions)
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<GetQuestionsDTO> cachedQuestions))
            {
                _logger.LogDebug("Random questions found in memory cache");
                return cachedQuestions;
            }

            // Get from database
            var result = await _questionService.GetRandomQuestionsAsync(count);
            
            if (result?.Any() == true)
            {
                // Cache for shorter time since random questions should change
                var shortCacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2),
                    Priority = CacheItemPriority.Normal
                };
                
                _memoryCache.Set(cacheKey, result, shortCacheOptions);
                _logger.LogDebug("Random questions cached successfully");
            }

            return result;
        }

        public async Task<ResponseBaseHttp<string>> CreateQuestionAsync(CreateQuestionDTO questionDto)
        {
            var result = await _questionService.CreateQuestionAsync(questionDto);
            
            if (string.IsNullOrEmpty(result.Result))
            {
                // Clear random questions cache when new question is added
                await InvalidateRandomQuestionsCache();
                _logger.LogDebug("Cache invalidated after creating new question");
            }
            
            return result;
        }

        public async Task<ResponseBaseHttp<string>> UpdateQuestionAsync(int id, CreateQuestionDTO questionDto)
        {
            var result = await _questionService.UpdateQuestionAsync(id, questionDto);
            
            if (string.IsNullOrEmpty(result.Result))
            {
                // Remove specific question from cache
                var cacheKey = $"question_{id}";
                _memoryCache.Remove(cacheKey);
                
                if (_distributedCache != null)
                {
                    try
                    {
                        await _distributedCache.RemoveAsync(cacheKey);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to remove question {Id} from distributed cache", id);
                    }
                }
                
                // Clear random questions cache
                await InvalidateRandomQuestionsCache();
                _logger.LogDebug("Cache invalidated after updating question {Id}", id);
            }
            
            return result;
        }

        public async Task<ResponseBaseHttp<string>> DeleteQuestionAsync(int id)
        {
            var result = await _questionService.DeleteQuestionAsync(id);
            
            if (string.IsNullOrEmpty(result.Result))
            {
                // Remove specific question from cache
                var cacheKey = $"question_{id}";
                _memoryCache.Remove(cacheKey);
                
                if (_distributedCache != null)
                {
                    try
                    {
                        await _distributedCache.RemoveAsync(cacheKey);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to remove question {Id} from distributed cache", id);
                    }
                }
                
                // Clear random questions cache
                await InvalidateRandomQuestionsCache();
                _logger.LogDebug("Cache invalidated after deleting question {Id}", id);
            }
            
            return result;
        }

        private async Task InvalidateRandomQuestionsCache()
        {
            // Remove common random question cache keys
            var cacheKeys = new[] { "random_questions_15", "random_questions_10", "random_questions_20" };
            
            foreach (var key in cacheKeys)
            {
                _memoryCache.Remove(key);
                
                if (_distributedCache != null)
                {
                    try
                    {
                        await _distributedCache.RemoveAsync(key);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to remove {CacheKey} from distributed cache", key);
                    }
                }
            }
        }
    }
}
