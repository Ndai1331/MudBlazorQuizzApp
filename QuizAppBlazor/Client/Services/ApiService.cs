using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using QuizAppBlazor.Client.DTOs;
using QuizAppBlazor.Client.HttpResponse;

namespace QuizAppBlazor.Client.Services
{
    /// <summary>
    /// API service implementation with improved error handling and logging
    /// </summary>
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        #region Question Methods

        public async Task<ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>> GetQuestionsAsync(string search = "", int skip = 0, int take = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Quizz/Question/list?question={Uri.EscapeDataString(search)}&skip={skip}&take={take}");
                return await HandleResponseAsync<ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting questions");
                return new ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>
                {
                    Result = new List<GetQuestionsDTO>()
                };
            }
        }

        public async Task<ResponseBaseHttp<GetQuestionsDTO>> GetQuestionByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Quizz/Question/{id}");
                return await HandleResponseAsync<ResponseBaseHttp<GetQuestionsDTO>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting question {Id}", id);
                return new ResponseBaseHttp<GetQuestionsDTO>
                {
                    Result = null!
                };
            }
        }

        public async Task<IEnumerable<GetQuestionsDTO>> GetRandomQuestionsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Quizz/Question/random");
                if (response.IsSuccessStatusCode)
                {
                    var questions = await response.Content.ReadFromJsonAsync<IEnumerable<GetQuestionsDTO>>(_jsonOptions);
                    return questions ?? new List<GetQuestionsDTO>();
                }
                
                _logger.LogWarning("Failed to get random questions. Status: {StatusCode}", response.StatusCode);
                return new List<GetQuestionsDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting random questions");
                return new List<GetQuestionsDTO>();
            }
        }

        public async Task<ResponseBaseHttp<string>> CreateQuestionAsync(CreateQuestionDTO question)
        {
            try
            {
                var json = JsonSerializer.Serialize(question, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Quizz/Question/create", content);
                return await HandleResponseAsync<ResponseBaseHttp<string>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                return new ResponseBaseHttp<string>
                {
                    Result = ex.Message
                };
            }
        }

        public async Task<ResponseBaseHttp<string>> UpdateQuestionAsync(int id, CreateQuestionDTO question)
        {
            try
            {
                var json = JsonSerializer.Serialize(question, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/Quizz/Question/{id}", content);
                return await HandleResponseAsync<ResponseBaseHttp<string>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question {Id}", id);
                return new ResponseBaseHttp<string>
                {
                    Result = ex.Message
                };
            }
        }

        public async Task<ResponseBaseHttp<string>> DeleteQuestionAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Quizz/Question/{id}");
                return await HandleResponseAsync<ResponseBaseHttp<string>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question {Id}", id);
                return new ResponseBaseHttp<string>
                {
                    Result = ex.Message
                };
            }
        }

        #endregion

        #region User Methods

        public async Task<UserAuthDto> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(loginDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Quizz/User/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserAuthDto>(_jsonOptions);
                    return result ?? new UserAuthDto { Message = "Invalid response format" };
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Login failed. Status: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
                
                return new UserAuthDto
                {
                    Message = "Đăng nhập thất bại"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return new UserAuthDto
                {
                    Message = "Lỗi kết nối"
                };
            }
        }

        public async Task<ResponseBaseHttp<string>> RegisterAsync(UserLoginDto registerDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(registerDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Quizz/User/register", content);
                return await HandleResponseAsync<ResponseBaseHttp<string>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return new ResponseBaseHttp<string>
                {
                    Result = ex.Message
                };
            }
        }

        public async Task<UserAuthDto> GetUserByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Quizz/User/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserAuthDto>(_jsonOptions);
                    return result ?? new UserAuthDto();
                }
                
                return new UserAuthDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {Id}", id);
                return new UserAuthDto();
            }
        }

        public async Task<ResponseBaseHttp<string>> UpdateUserAsync(UserLoginDto userDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(userDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Quizz/User/update", content);
                return await HandleResponseAsync<ResponseBaseHttp<string>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return new ResponseBaseHttp<string>
                {
                    Result = ex.Message
                };
            }
        }

        #endregion

        #region Score Methods

        public async Task<ResponseBaseHttp<IEnumerable<GetScoreByAuthorDTO>>> GetScoresAsync(string userNickname = "", int skip = 0, int take = 10, bool isAdmin = false)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Quizz/Score?userNickname={Uri.EscapeDataString(userNickname)}&skip={skip}&take={take}&isAdmin={isAdmin}");
                return await HandleResponseAsync<ResponseBaseHttp<IEnumerable<GetScoreByAuthorDTO>>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting scores");
                return new ResponseBaseHttp<IEnumerable<GetScoreByAuthorDTO>>
                {
                    Result = new List<GetScoreByAuthorDTO>()
                };
            }
        }

        public async Task<ResponseBaseHttp<string>> SubmitScoreAsync(UserScoreDTO score)
        {
            try
            {
                var json = JsonSerializer.Serialize(score, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Quizz/Score/post", content);
                return await HandleResponseAsync<ResponseBaseHttp<string>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting score");
                return new ResponseBaseHttp<string>
                {
                    Result = ex.Message
                };
            }
        }

        #endregion

        #region Quiz Methods (New)

        public async Task<QuizDTO> StartQuizAsync(CreateQuizDTO createQuizDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(createQuizDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Quizz/Quiz/start", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<QuizDTO>(_jsonOptions);
                    return result ?? new QuizDTO();
                }
                
                return new QuizDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting quiz");
                return new QuizDTO();
            }
        }

        public async Task<ResponseBaseHttp<string>> SubmitQuizAnswerAsync(SubmitQuizAnswerDTO answerDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(answerDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Quizz/Quiz/answer", content);
                return await HandleResponseAsync<ResponseBaseHttp<string>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting quiz answer");
                return new ResponseBaseHttp<string>
                {
                    Result = ex.Message
                };
            }
        }

        public async Task<QuizDTO> CompleteQuizAsync(int quizId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Quizz/Quiz/{quizId}/complete", null);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<QuizDTO>(_jsonOptions);
                    return result ?? new QuizDTO();
                }
                
                return new QuizDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing quiz {QuizId}", quizId);
                return new QuizDTO();
            }
        }

        public async Task<ResponseBaseHttp<IEnumerable<QuizDTO>>> GetUserQuizzesAsync(string userId, int skip = 0, int take = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Quizz/Quiz/user/{userId}?skip={skip}&take={take}");
                return await HandleResponseAsync<ResponseBaseHttp<IEnumerable<QuizDTO>>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user quizzes for {UserId}", userId);
                return new ResponseBaseHttp<IEnumerable<QuizDTO>>
                {
                    Result = new List<QuizDTO>()
                };
            }
        }

        #endregion

        #region Helper Methods

        private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response) where T : new()
        {
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                        return result ?? new T();
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("API call failed. Status: {StatusCode}, Content: {Content}", 
                        response.StatusCode, errorContent);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling response");
            }

            return new T();
        }

        #endregion
    }
}
