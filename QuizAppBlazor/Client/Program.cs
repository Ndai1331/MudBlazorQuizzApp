using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QuizAppBlazor.Client.Providers;
using MudBlazor.Services;

namespace QuizAppBlazor.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddBlazoredLocalStorageAsSingleton();
            builder.Services.AddScoped<
                AuthenticationStateProvider,
                CustomAuthenticationStateProvider
            >();
            
            // Register API service
            builder.Services.AddScoped<QuizAppBlazor.Client.Services.IApiService, QuizAppBlazor.Client.Services.ApiService>();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            
            // Configure HttpClient with better error handling
            builder.Services.AddScoped<HttpClient>(sp => 
            {
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri("https://localhost:8080"),
                    Timeout = TimeSpan.FromSeconds(30)
                };
                
                // Add default headers
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "QuizApp-Blazor/1.0");
                
                return httpClient;
            });
            
            // Add MudBlazor with configuration
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomLeft;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 10000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = MudBlazor.Variant.Filled;
            });
            await builder.Build().RunAsync();
        }
    }
}
