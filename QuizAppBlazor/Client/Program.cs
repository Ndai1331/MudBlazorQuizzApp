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

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<HttpClient>(sp => new HttpClient
            {
                BaseAddress = new Uri("https://localhost:8080")
                // BaseAddress = new Uri("https://api-test.gemb.club")
            });
            builder.Services.AddMudServices();
            await builder.Build().RunAsync();
        }
    }
}
