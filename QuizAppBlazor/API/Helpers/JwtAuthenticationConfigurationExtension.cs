﻿using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
namespace QuizAppBlazor.API.Helpers
{
	public static class JwtAuthenticationConfigurationExtension
	{
        public static void AddJwtAutheticationConfiguration(
           this IServiceCollection services,
           JwtSettings settings)
        {
            // Register Jwt as the Authentication service
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters =
              new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(settings.Key)),
                  ValidateIssuer = true,
                  ValidIssuer = settings.Issuer,

                  ValidateAudience = true,
                  ValidAudience = settings.Audience,

                  ValidateLifetime = true,
                  ClockSkew = TimeSpan.FromMinutes(
                         settings.MinutesToExpiration)
              };
                jwtBearerOptions.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.SecurityToken is JsonWebToken accessToken)
                        {
                            var userName = accessToken.Claims.FirstOrDefault(a => a.Type == Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub)?.Value;
                            var email = accessToken.Claims.FirstOrDefault(a => a.Type == "Email")?.Value;
                            context.HttpContext.Items["Id"] = userName;
                            var userInfoToken = context.HttpContext.RequestServices.GetRequiredService<UserInfoToken>();
                            userInfoToken.Id = userName;
                            userInfoToken.Email = email;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();
        }
    }
}

