using System;
using Microsoft.IdentityModel.Tokens;
using QuizAppBlazor.API.Models;
using QuizAppBlazor.API.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuizAppBlazor.API.Helpers
{
    public class JwtHelper
    {
        public static UserAuthDto BuildUserAuthObject(ApplicationUser appUser, JwtSettings _settings)
        {
            Guid userId = Guid.Parse(appUser.Id);
            UserAuthDto ret = new UserAuthDto
            {
                Id = userId,
                Email = appUser?.Email,
                Role = appUser.Role,
                IsAuthenticated = true,
            };
            ret.BearerToken = BuildJwtToken(ret, new List<Claim>(), userId, _settings);
            return ret;
        }

        protected static string BuildJwtToken(UserAuthDto authUser, IList<Claim> claims, Guid Id, JwtSettings _settings)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key)
            );
            claims.Add(
                new Claim(
                    Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub.ToString(),
                    Id.ToString()
                )
            );
            claims.Add(new Claim("Email", authUser.Email));
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_settings.MinutesToExpiration),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            // Create a string representation of the Jwt token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

  



}

