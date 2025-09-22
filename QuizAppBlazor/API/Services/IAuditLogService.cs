namespace QuizAppBlazor.API.Services
{
    /// <summary>
    /// Service for logging user activities and system events
    /// </summary>
    public interface IAuditLogService
    {
        Task LogUserActionAsync(string userId, string action, string details, string? ipAddress = null);
        Task LogSystemEventAsync(string eventType, string message, string? details = null);
        Task LogQuizActivityAsync(string userId, int quizId, string activity, string details);
        Task LogQuestionActivityAsync(string userId, int questionId, string activity, string details);
        Task LogSecurityEventAsync(string eventType, string userId, string details, string? ipAddress = null);
    }
}
