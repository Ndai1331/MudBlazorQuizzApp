using System.Text.Json;

namespace QuizAppBlazor.API.Services
{
    /// <summary>
    /// Implementation of audit logging service
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(ILogger<AuditLogService> logger)
        {
            _logger = logger;
        }

        public async Task LogUserActionAsync(string userId, string action, string details, string? ipAddress = null)
        {
            var logData = new
            {
                UserId = userId,
                Action = action,
                Details = details,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow,
                Type = "UserAction"
            };

            _logger.LogInformation("User Action: {LogData}", JsonSerializer.Serialize(logData));
            await Task.CompletedTask;
        }

        public async Task LogSystemEventAsync(string eventType, string message, string? details = null)
        {
            var logData = new
            {
                EventType = eventType,
                Message = message,
                Details = details,
                Timestamp = DateTime.UtcNow,
                Type = "SystemEvent"
            };

            _logger.LogInformation("System Event: {LogData}", JsonSerializer.Serialize(logData));
            await Task.CompletedTask;
        }

        public async Task LogQuizActivityAsync(string userId, int quizId, string activity, string details)
        {
            var logData = new
            {
                UserId = userId,
                QuizId = quizId,
                Activity = activity,
                Details = details,
                Timestamp = DateTime.UtcNow,
                Type = "QuizActivity"
            };

            _logger.LogInformation("Quiz Activity: {LogData}", JsonSerializer.Serialize(logData));
            await Task.CompletedTask;
        }

        public async Task LogQuestionActivityAsync(string userId, int questionId, string activity, string details)
        {
            var logData = new
            {
                UserId = userId,
                QuestionId = questionId,
                Activity = activity,
                Details = details,
                Timestamp = DateTime.UtcNow,
                Type = "QuestionActivity"
            };

            _logger.LogInformation("Question Activity: {LogData}", JsonSerializer.Serialize(logData));
            await Task.CompletedTask;
        }

        public async Task LogSecurityEventAsync(string eventType, string userId, string details, string? ipAddress = null)
        {
            var logData = new
            {
                EventType = eventType,
                UserId = userId,
                Details = details,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow,
                Type = "SecurityEvent"
            };

            _logger.LogWarning("Security Event: {LogData}", JsonSerializer.Serialize(logData));
            await Task.CompletedTask;
        }
    }
}
