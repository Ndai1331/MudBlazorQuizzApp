using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using QuizAppBlazor.Client.DTOs;

namespace QuizAppBlazor.Client.Shared
{
    public abstract class BaseBlazorPage : ComponentBase, IDisposable
    {
        public UserAuthDto UserLoggedIn { set; get; }

        [CascadingParameter]
        public Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        public IJSRuntime _jsRuntime { get; set; }

        [Inject]
        public ILocalStorageService _localStorageService { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public HttpClient Http { get; set; }

        protected override async Task OnInitializedAsync()
        {
            UserLoggedIn = await GetUserLogin();
            Http = await AttachToken();
        }

        public async Task<UserAuthDto> GetUserLogin()
        {
            try
            {
                var token = await _localStorageService.GetItemAsync<string>("token");
                var role = await _localStorageService.GetItemAsync<string>("role");
                var email = await _localStorageService.GetItemAsync<string>("email");
                var id = await _localStorageService.GetItemAsync<string>("id");
                Guid userId;
                if (!Guid.TryParse(id, out userId))
                {
                    userId = Guid.Empty;
                }
                var user = new UserAuthDto()
                {
                    BearerToken = token,
                    Email = email,
                    Role = role,
                    Id = userId,
                    IsAuthenticated = userId != Guid.Empty ? true : false
                };
                return user;
            }
            catch
            {
                return null;
            }
        }

        public void Dispose() { }

        public async Task<HttpClient> AttachToken()
        {
            var token = await _localStorageService.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                Http.DefaultRequestHeaders.Clear();
                Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token
                );
            }

            return Http;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await AuthState;
            return authState?.User?.Identity?.IsAuthenticated ?? false;
        }

        public async Task LogoutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "token");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "role");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "email");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "id");
            UserLoggedIn = null;
            Navigation.NavigateTo($"/login");
        }
    }
}
