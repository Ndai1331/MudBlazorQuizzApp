using Microsoft.AspNetCore.Identity;
using QuizAppBlazor.API.Data;
using QuizAppBlazor.API.Models;

namespace QuizAppBlazor.API.Services
{
    /// <summary>
    /// Service to seed initial data including admin user
    /// </summary>
    public class SeedDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SeedDataService> _logger;

        public SeedDataService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<SeedDataService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Seed initial admin user and sample data
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting data seeding...");

                // Ensure database is created
                await _context.Database.EnsureCreatedAsync();

                // Seed admin user
                await SeedAdminUserAsync();

                // Seed sample questions
                await SeedSampleQuestionsAsync();

                _logger.LogInformation("Data seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during data seeding");
                throw;
            }
        }

        /// <summary>
        /// Create admin user if not exists
        /// </summary>
        private async Task SeedAdminUserAsync()
        {
            const string adminEmail = "admin@quizapp.com";
            const string adminPassword = "Abc@123!!!";
            const string adminNickname = "Admin";

            // Check if admin user already exists
            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null)
            {
                _logger.LogInformation("Admin user already exists: {Email}", adminEmail);
                return;
            }

            // Create admin user
            var adminUser = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                Nickname = adminNickname,
                Role = "Admin",
                EmailConfirmed = true // Skip email confirmation for admin
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("Admin user created successfully: {Email}", adminEmail);
                _logger.LogInformation("Admin login credentials - Email: {Email}, Password: {Password}", 
                    adminEmail, adminPassword);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create admin user: {Errors}", errors);
                throw new Exception($"Failed to create admin user: {errors}");
            }
        }

        /// <summary>
        /// Seed sample questions for testing
        /// </summary>
        private async Task SeedSampleQuestionsAsync()
        {
            // Check if questions already exist
            if (_context.Questions.Any())
            {
                _logger.LogInformation("Questions already exist, skipping sample questions seeding");
                return;
            }

            var sampleQuestions = new List<QuestionModel>
            {
                new QuestionModel
                {
                    Question = "Thủ đô của Việt Nam là gì?",
                    CorrectAnswer = "Hà Nội",
                    Alternativ2 = "Hồ Chí Minh",
                    Alternativ3 = "Đà Nẵng",
                    Alternativ4 = "Cần Thơ",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 30
                },
                new QuestionModel
                {
                    Question = "2 + 2 = ?",
                    CorrectAnswer = "4",
                    Alternativ2 = "3",
                    Alternativ3 = "5",
                    Alternativ4 = "6",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 15
                },
                new QuestionModel
                {
                    Question = "Ngôn ngữ lập trình nào được sử dụng để phát triển ứng dụng này?",
                    CorrectAnswer = "C#",
                    Alternativ2 = "Java",
                    Alternativ3 = "Python",
                    Alternativ4 = "JavaScript",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 45
                },
                new QuestionModel
                {
                    Question = "Framework nào được sử dụng cho frontend của ứng dụng này?",
                    CorrectAnswer = "Blazor",
                    Alternativ2 = "React",
                    Alternativ3 = "Vue.js",
                    Alternativ4 = "Angular",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 45
                },
                new QuestionModel
                {
                    Question = "Database nào được sử dụng trong dự án này?",
                    CorrectAnswer = "PostgreSQL",
                    Alternativ2 = "MySQL",
                    Alternativ3 = "SQL Server",
                    Alternativ4 = "MongoDB",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 30
                },
                new QuestionModel
                {
                    Question = "Mô tả ngắn gọn về ASP.NET Core",
                    CorrectAnswer = "Framework web cross-platform của Microsoft",
                    Alternativ2 = "", // Empty string instead of null
                    Alternativ3 = "", // Empty string instead of null
                    Alternativ4 = "", // Empty string instead of null
                    IsTextInput = true,
                    HasTimeLimit = true,
                    TimeLimit = 120
                },
                new QuestionModel
                {
                    Question = "Năm nào Việt Nam gia nhập ASEAN?",
                    CorrectAnswer = "1995",
                    Alternativ2 = "1990",
                    Alternativ3 = "1998",
                    Alternativ4 = "2000",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 45
                },
                new QuestionModel
                {
                    Question = "Đâu là ngôn ngữ lập trình hướng đối tượng?",
                    CorrectAnswer = "C#",
                    Alternativ2 = "HTML",
                    Alternativ3 = "CSS",
                    Alternativ4 = "SQL",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 30
                },
                new QuestionModel
                {
                    Question = "HTTP status code 404 có nghĩa là gì?",
                    CorrectAnswer = "Not Found",
                    Alternativ2 = "Bad Request",
                    Alternativ3 = "Internal Server Error",
                    Alternativ4 = "Unauthorized",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 30
                },
                new QuestionModel
                {
                    Question = "MudBlazor là gì?",
                    CorrectAnswer = "UI Component Library cho Blazor",
                    Alternativ2 = "Database ORM",
                    Alternativ3 = "Web Server",
                    Alternativ4 = "Programming Language",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 45
                },
                new QuestionModel
                {
                    Question = "JWT viết tắt của từ gì?",
                    CorrectAnswer = "JSON Web Token",
                    Alternativ2 = "Java Web Technology",
                    Alternativ3 = "JavaScript Web Tool",
                    Alternativ4 = "Just Web Token",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 45
                },
                new QuestionModel
                {
                    Question = "Entity Framework Core là gì?",
                    CorrectAnswer = "Object-Relational Mapping (ORM) framework",
                    Alternativ2 = "Web framework",
                    Alternativ3 = "UI framework",
                    Alternativ4 = "Testing framework",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 60
                },
                new QuestionModel
                {
                    Question = "Docker được sử dụng để làm gì?",
                    CorrectAnswer = "Containerization",
                    Alternativ2 = "Database management",
                    Alternativ3 = "Code editing",
                    Alternativ4 = "Web hosting",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 45
                },
                new QuestionModel
                {
                    Question = "CORS viết tắt của từ gì?",
                    CorrectAnswer = "Cross-Origin Resource Sharing",
                    Alternativ2 = "Common Object Request Syntax",
                    Alternativ3 = "Client-Origin Resource Security",
                    Alternativ4 = "Cross-Object Resource System",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 60
                },
                new QuestionModel
                {
                    Question = "Dependency Injection có lợi ích gì?",
                    CorrectAnswer = "Loose coupling và testability",
                    Alternativ2 = "Tăng performance",
                    Alternativ3 = "Giảm memory usage",
                    Alternativ4 = "Tăng security",
                    IsTextInput = false,
                    HasTimeLimit = true,
                    TimeLimit = 60
                }
            };

            _context.Questions.AddRange(sampleQuestions);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} sample questions", sampleQuestions.Count);
        }

        /// <summary>
        /// Check if seeding is needed
        /// </summary>
        public async Task<bool> IsSeedingNeededAsync()
        {
            var hasUsers = _context.Users.Any();
            var hasQuestions = _context.Questions.Any();

            return !hasUsers || !hasQuestions;
        }

        /// <summary>
        /// Create additional test users
        /// </summary>
        public async Task SeedTestUsersAsync()
        {
            var testUsers = new[]
            {
                new { Email = "user1@test.com", Nickname = "TestUser1", Password = "Test@123!", Role = "User" },
                new { Email = "user2@test.com", Nickname = "TestUser2", Password = "Test@123!", Role = "User" },
                new { Email = "teacher@test.com", Nickname = "Teacher", Password = "Teacher@123!", Role = "Teacher" }
            };

            foreach (var userData in testUsers)
            {
                var existingUser = await _userManager.FindByEmailAsync(userData.Email);
                if (existingUser != null) continue;

                var user = new ApplicationUser
                {
                    Email = userData.Email,
                    UserName = userData.Email,
                    Nickname = userData.Nickname,
                    Role = userData.Role,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, userData.Password);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Test user created: {Email}", userData.Email);
                }
                else
                {
                    _logger.LogWarning("Failed to create test user {Email}: {Errors}", 
                        userData.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
