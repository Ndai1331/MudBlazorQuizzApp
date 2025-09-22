using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using QuizAppBlazor.Client.DTOs;
using QuizAppBlazor.Client.Services;

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

        [Inject]
        public IApiService ApiService { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        protected bool IsLoading { get; set; } = false;

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
            await _localStorageService.RemoveItemAsync("token");
            await _localStorageService.RemoveItemAsync("role");
            await _localStorageService.RemoveItemAsync("email");
            await _localStorageService.RemoveItemAsync("id");
            await _localStorageService.RemoveItemAsync("nickname");
            
            UserLoggedIn = null;
            Http.DefaultRequestHeaders.Authorization = null;
            
            Snackbar.Add("Đã đăng xuất thành công", Severity.Info);
            Navigation.NavigateTo("/login", true);
        }

        protected async Task ShowErrorAsync(string message)
        {
            Snackbar.Add(message, Severity.Error);
        }

        protected async Task ShowSuccessAsync(string message)
        {
            Snackbar.Add(message, Severity.Success);
        }

        protected async Task ShowInfoAsync(string message)
        {
            Snackbar.Add(message, Severity.Info);
        }

        protected async Task ShowWarningAsync(string message)
        {
            Snackbar.Add(message, Severity.Warning);
        }

        protected async Task<bool> ConfirmAsync(string title, string message)
        {
            // Simple confirmation using JavaScript for now
            return await _jsRuntime.InvokeAsync<bool>("confirm", $"{title}\n\n{message}");
        }
    }
}
