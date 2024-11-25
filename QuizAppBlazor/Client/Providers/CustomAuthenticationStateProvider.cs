using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using QuizAppBlazor.Client.DTOs;

namespace QuizAppBlazor.Client.Providers
{
    public class CustomAuthenticationStateProvider(ILocalStorageService _localStorageService)
        : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsync<string>("token");
            var id = await _localStorageService.GetItemAsync<string>("id");
            var email = await _localStorageService.GetItemAsync<string>("email");
            var role = await _localStorageService.GetItemAsync<string>("role");

            ClaimsIdentity identity;
            Guid userId;

            if (!string.IsNullOrEmpty(token) && Guid.TryParse(id, out userId))
            {
                UserAuthDto user = new UserAuthDto()
                {
                    Id = userId,
                    BearerToken = token,
                    Email = email,
                    Role = role
                };
                identity = GetClaimsIdentity(user);
            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var claimsPrincipal = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorageService.RemoveItemAsync("token");
            await _localStorageService.RemoveItemAsync("id");
            await _localStorageService.RemoveItemAsync("isAdmin");
            await _localStorageService.RemoveItemAsync("email");
            await _localStorageService.RemoveItemAsync("role");

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private ClaimsIdentity GetClaimsIdentity(UserAuthDto user)
        {
            var claimsIdentity = new ClaimsIdentity();

            if (user != null)
            {
                claimsIdentity = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Role, user.Role)
                    },
                    "apiauth_type"
                );
            }

            return claimsIdentity;
        }

        public static ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = "PTCUsers";
            validationParameters.ValidIssuer = "http://localhost:5000";
            validationParameters.IssuerSigningKey =
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("This*Is&A!Long)Key(For%Creating@A$SymmetricKey")
                );

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(
                jwtToken,
                validationParameters,
                out validatedToken
            );

            return principal;
        }

        public async Task MarkUserAsAuthenticated(UserAuthDto user)
        {
            var claimsPrincipal = ValidateToken(user.BearerToken);
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(claimsPrincipal))
            );
        }
    }
}
