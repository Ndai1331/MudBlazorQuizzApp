using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuizAppBlazor.API.Data;
using QuizAppBlazor.API.Helpers;
using QuizAppBlazor.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Expand environment variables in configuration
builder.Configuration.AddEnvironmentVariables();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
});

// Add file logging if configured
var logPath = Environment.ExpandEnvironmentVariables(
    builder.Configuration["Logging:File:Path"] ?? "logs/quizapp-.txt"
);
if (!string.IsNullOrEmpty(logPath))
{
    Directory.CreateDirectory(Path.GetDirectoryName(logPath) ?? "logs");
}

// Configure structured logging
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Handle environment variable format ${VAR:default}
if (connectionString.StartsWith("${") && connectionString.Contains(":"))
{
    var envVarName = connectionString.Substring(2, connectionString.IndexOf(':') - 2);
    var defaultValue = connectionString.Substring(connectionString.IndexOf(':') + 1);
    defaultValue = defaultValue.EndsWith("}") ? defaultValue.Substring(0, defaultValue.Length - 1) : defaultValue;
    
    connectionString = Environment.GetEnvironmentVariable(envVarName) ?? defaultValue;
}

// Add services to the container.
JwtSettings settings;
settings = GetJwtSettings(builder);
builder.Services.AddSingleton<JwtSettings>(settings);
builder.Services.AddScoped<UserInfoToken>(c => new UserInfoToken() { Id = "" });

builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Add caching
builder.Services.AddMemoryCache();

// Redis cache disabled due to connection timeout issues
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
//     options.Configuration = ExpandConfigValue(redisConnection);
// });

// Register services
builder.Services.AddScoped<QuizAppBlazor.API.Services.QuestionService>();
builder.Services.AddScoped<QuizAppBlazor.API.Services.IScoreService, QuizAppBlazor.API.Services.ScoreService>();
builder.Services.AddScoped<QuizAppBlazor.API.Services.IAuditLogService, QuizAppBlazor.API.Services.AuditLogService>();
builder.Services.AddScoped<QuizAppBlazor.API.Services.SeedDataService>();

// Register cached service as decorator - only memory cache
builder.Services.AddScoped<QuizAppBlazor.API.Services.IQuestionService>(serviceProvider =>
{
    var baseService = serviceProvider.GetRequiredService<QuizAppBlazor.API.Services.QuestionService>();
    var memoryCache = serviceProvider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
    var logger = serviceProvider.GetRequiredService<ILogger<QuizAppBlazor.API.Services.CachedQuestionService>>();
    
    // Pass null for distributed cache since we disabled Redis
    return new QuizAppBlazor.API.Services.CachedQuestionService(baseService, memoryCache, null, logger);
});

builder
    .Services.AddDefaultIdentity<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings - strengthen security requirements
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 3;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // User settings
    options.User.RequireUniqueEmail = true;
});
builder.Services.AddJwtAutheticationConfiguration(settings);
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AutomaticAuthentication = false;
});
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "BlazorCors",
        policy =>
        {
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                ?? new[] { "https://localhost:7000", "http://localhost:5000", "https://localhost:7208" };
            
            policy.WithOrigins(allowedOrigins)
                  .WithHeaders("Content-Type", "Authorization")
                  .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                  .AllowCredentials();
        }
    );
});

// Add builder.Services to the container.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

//Configure external authentication cookies
builder.Services.ConfigureExternalCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddControllers(options =>
{
    // Enable automatic model validation
    options.ModelValidatorProviders.Clear();
})
.ConfigureApiBehaviorOptions(options =>
{
    // Customize model validation response
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(e => e.Value.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var response = new QuizAppBlazor.API.HttpResponse.ResponseBaseHttp<List<string>>
        {
            Result = errors
        };

        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add JWT Authentication
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description =
                "Please enter into field the word 'Bearer' followed by a space and the JWT value",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        }
    );
});

var app = builder.Build();

// Use request logging middleware (before exception handling)
app.UseMiddleware<QuizAppBlazor.API.Middleware.RequestLoggingMiddleware>();

// Use global exception handling middleware
app.UseMiddleware<QuizAppBlazor.API.Middleware.GlobalExceptionMiddleware>();

app.UseCors("BlazorCors");

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
});

app.UseAuthorization();

app.MapControllers();

// Add a simple test endpoint
app.MapGet("/", () => "API is running! Go to /swagger for API documentation");

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var seedService = scope.ServiceProvider.GetRequiredService<QuizAppBlazor.API.Services.SeedDataService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        if (await seedService.IsSeedingNeededAsync())
        {
            logger.LogInformation("Seeding initial data...");
            await seedService.SeedAsync();
            
            // Optionally seed test users in development environment
            if (app.Environment.IsDevelopment())
            {
                await seedService.SeedTestUsersAsync();
                logger.LogInformation("Test users seeded for development environment");
            }
        }
        else
        {
            logger.LogInformation("Data seeding not needed - data already exists");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during data seeding");
        // Don't throw - let the app continue running
    }
}

app.Run();

static JwtSettings GetJwtSettings(WebApplicationBuilder builder)
{
    JwtSettings settings = new JwtSettings();

    settings.Key = ExpandConfigValue(builder.Configuration["JwtSettings:key"] ?? throw new InvalidOperationException("JWT Key not configured"));
    settings.Audience = ExpandConfigValue(builder.Configuration["JwtSettings:audience"] ?? throw new InvalidOperationException("JWT Audience not configured"));
    settings.Issuer = ExpandConfigValue(builder.Configuration["JwtSettings:issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured"));
    settings.MinutesToExpiration = Convert.ToInt32(ExpandConfigValue(builder.Configuration["JwtSettings:minutesToExpiration"] ?? "720"));

    return settings;
}

static string ExpandConfigValue(string configValue)
{
    if (configValue.StartsWith("${") && configValue.Contains(":"))
    {
        var envVarName = configValue.Substring(2, configValue.IndexOf(':') - 2);
        var defaultValue = configValue.Substring(configValue.IndexOf(':') + 1);
        defaultValue = defaultValue.EndsWith("}") ? defaultValue.Substring(0, defaultValue.Length - 1) : defaultValue;
        
        return Environment.GetEnvironmentVariable(envVarName) ?? defaultValue;
    }
    return configValue;
}
