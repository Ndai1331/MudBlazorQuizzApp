using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuizAppBlazor.API.Data;
using QuizAppBlazor.API.Helpers;
using QuizAppBlazor.API.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'ApplicationDbContextConnection' not found."
    );

// Add services to the container.
JwtSettings settings;
settings = GetJwtSettings(builder);
builder.Services.AddSingleton<JwtSettings>(settings);
builder.Services.AddScoped<UserInfoToken>(c => new UserInfoToken() { Id = "" });

builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder
    .Services.AddDefaultIdentity<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
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
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
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
builder.Services.AddControllers();

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
app.UseCors("BlazorCors");

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static JwtSettings GetJwtSettings(WebApplicationBuilder builder)
{
    JwtSettings settings = new JwtSettings();

    settings.Key = builder.Configuration["JwtSettings:key"];
    settings.Audience = builder.Configuration["JwtSettings:audience"];
    settings.Issuer = builder.Configuration["JwtSettings:issuer"];
    settings.MinutesToExpiration = Convert.ToInt32(
        builder.Configuration["JwtSettings:minutesToExpiration"]
    );

    return settings;
}
