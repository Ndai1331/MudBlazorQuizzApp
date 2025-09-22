using Microsoft.EntityFrameworkCore;
using QuizAppBlazor.API.Data;
using QuizAppBlazor.API.Models;
using QuizAppBlazor.API.Models.Entities;

namespace QuizAppBlazor.API.Services
{
    /// <summary>
    /// Service to migrate data from old structure to new improved structure
    /// </summary>
    public class DataMigrationService
    {
        private readonly ApplicationDbContext _oldContext;
        private readonly ApplicationDbContextV2 _newContext;
        private readonly ILogger<DataMigrationService> _logger;

        public DataMigrationService(
            ApplicationDbContext oldContext,
            ApplicationDbContextV2 newContext,
            ILogger<DataMigrationService> logger)
        {
            _oldContext = oldContext;
            _newContext = newContext;
            _logger = logger;
        }

        /// <summary>
        /// Migrate questions from old structure to new structure
        /// </summary>
        public async Task MigrateQuestionsAsync()
        {
            try
            {
                _logger.LogInformation("Starting question migration...");

                var oldQuestions = await _oldContext.Questions.ToListAsync();
                var migratedCount = 0;

                foreach (var oldQuestion in oldQuestions)
                {
                    // Check if question already migrated
                    var existingQuestion = await _newContext.Questions
                        .FirstOrDefaultAsync(q => q.Content == oldQuestion.Question);

                    if (existingQuestion != null)
                    {
                        _logger.LogDebug("Question already migrated: {QuestionId}", oldQuestion.Id);
                        continue;
                    }

                    // Create new question
                    var newQuestion = new Question
                    {
                        Content = oldQuestion.Question,
                        CorrectAnswer = oldQuestion.CorrectAnswer,
                        IsTextInput = oldQuestion.IsTextInput ?? false,
                        HasTimeLimit = oldQuestion.HasTimeLimit ?? true,
                        TimeLimit = oldQuestion.TimeLimit ?? 60,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    _newContext.Questions.Add(newQuestion);
                    await _newContext.SaveChangesAsync();

                    // Migrate question options for multiple choice questions
                    if (!newQuestion.IsTextInput)
                    {
                        var options = new List<QuestionOption>();

                        // Add correct answer as an option
                        options.Add(new QuestionOption
                        {
                            QuestionId = newQuestion.Id,
                            Content = oldQuestion.CorrectAnswer,
                            IsCorrect = true,
                            DisplayOrder = 1,
                            CreatedAt = DateTime.UtcNow
                        });

                        // Add alternative answers
                        if (!string.IsNullOrWhiteSpace(oldQuestion.Alternativ2))
                        {
                            options.Add(new QuestionOption
                            {
                                QuestionId = newQuestion.Id,
                                Content = oldQuestion.Alternativ2,
                                IsCorrect = false,
                                DisplayOrder = 2,
                                CreatedAt = DateTime.UtcNow
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(oldQuestion.Alternativ3))
                        {
                            options.Add(new QuestionOption
                            {
                                QuestionId = newQuestion.Id,
                                Content = oldQuestion.Alternativ3,
                                IsCorrect = false,
                                DisplayOrder = 3,
                                CreatedAt = DateTime.UtcNow
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(oldQuestion.Alternativ4))
                        {
                            options.Add(new QuestionOption
                            {
                                QuestionId = newQuestion.Id,
                                Content = oldQuestion.Alternativ4,
                                IsCorrect = false,
                                DisplayOrder = 4,
                                CreatedAt = DateTime.UtcNow
                            });
                        }

                        _newContext.QuestionOptions.AddRange(options);
                    }

                    // Migrate media if exists
                    if (!string.IsNullOrWhiteSpace(oldQuestion.ImageVideo))
                    {
                        var mediaType = MediaType.Image; // Default

                        if (oldQuestion.IsVideo == true)
                            mediaType = MediaType.Video;
                        else if (oldQuestion.IsYoutubeVideo == true)
                            mediaType = MediaType.YouTube;
                        else if (oldQuestion.IsImage == true)
                            mediaType = MediaType.Image;

                        var media = new QuestionMedia
                        {
                            QuestionId = newQuestion.Id,
                            Url = oldQuestion.ImageVideo,
                            Type = mediaType,
                            CreatedAt = DateTime.UtcNow
                        };

                        _newContext.QuestionMedia.Add(media);
                    }

                    await _newContext.SaveChangesAsync();
                    migratedCount++;

                    _logger.LogDebug("Migrated question: {OldId} -> {NewId}", oldQuestion.Id, newQuestion.Id);
                }

                _logger.LogInformation("Question migration completed. Migrated {Count} questions", migratedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during question migration");
                throw;
            }
        }

        /// <summary>
        /// Migrate scores from old structure to new quiz structure
        /// </summary>
        public async Task MigrateScoresAsync()
        {
            try
            {
                _logger.LogInformation("Starting score migration...");

                var oldScores = await _oldContext.Score.ToListAsync();
                var migratedCount = 0;

                foreach (var oldScore in oldScores)
                {
                    // Check if score already migrated
                    var existingQuiz = await _newContext.Quizzes
                        .FirstOrDefaultAsync(q => q.UserId == oldScore.UserId && 
                                                 q.StartedAt.Date == oldScore.Date.Date &&
                                                 q.CorrectAnswers == oldScore.CorrectAnswers);

                    if (existingQuiz != null)
                    {
                        _logger.LogDebug("Score already migrated: {ScoreId}", oldScore.Id);
                        continue;
                    }

                    // Parse old score data
                    var questions = oldScore.Questions?.Split('|') ?? Array.Empty<string>();
                    var answers = oldScore.Answers?.Split('|') ?? Array.Empty<string>();
                    var corrects = oldScore.Corrects?.Split('|') ?? Array.Empty<string>();

                    // Create new quiz
                    var quiz = new Quiz
                    {
                        UserId = oldScore.UserId,
                        StartedAt = oldScore.Date,
                        CompletedAt = oldScore.Date.AddMinutes(15), // Estimate completion time
                        TotalQuestions = questions.Length,
                        CorrectAnswers = oldScore.CorrectAnswers,
                        Status = QuizStatus.Completed
                    };

                    quiz.Complete();
                    _newContext.Quizzes.Add(quiz);
                    await _newContext.SaveChangesAsync();

                    // Create quiz question answers
                    for (int i = 0; i < questions.Length && i < answers.Length && i < corrects.Length; i++)
                    {
                        var questionAnswer = new QuizQuestionAnswer
                        {
                            QuizId = quiz.Id,
                            QuestionId = 0, // We don't have the exact question ID mapping
                            UserAnswer = answers[i],
                            CorrectAnswer = corrects[i],
                            IsCorrect = string.Equals(answers[i]?.Trim(), corrects[i]?.Trim(), StringComparison.OrdinalIgnoreCase),
                            AnsweredAt = oldScore.Date.AddMinutes(i), // Estimate answer times
                            QuestionOrder = i + 1
                        };

                        _newContext.QuizQuestionAnswers.Add(questionAnswer);
                    }

                    await _newContext.SaveChangesAsync();
                    migratedCount++;

                    _logger.LogDebug("Migrated score: {OldId} -> {NewId}", oldScore.Id, quiz.Id);
                }

                _logger.LogInformation("Score migration completed. Migrated {Count} scores", migratedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during score migration");
                throw;
            }
        }

        /// <summary>
        /// Run full migration process
        /// </summary>
        public async Task RunFullMigrationAsync()
        {
            _logger.LogInformation("Starting full data migration...");

            await MigrateQuestionsAsync();
            await MigrateScoresAsync();

            _logger.LogInformation("Full data migration completed successfully");
        }

        /// <summary>
        /// Check if migration is needed
        /// </summary>
        public async Task<bool> IsMigrationNeededAsync()
        {
            var hasOldQuestions = await _oldContext.Questions.AnyAsync();
            var hasNewQuestions = await _newContext.Questions.AnyAsync();

            return hasOldQuestions && !hasNewQuestions;
        }
    }
}
