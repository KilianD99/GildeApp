using Mvc.GildeApp.mvc.Services;

namespace Mvc.GildeApp.mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // A typed client, not a hand-rolled HttpClient: the web app talks to the API
            // over HTTP exactly like the mobile app does, so the two cannot drift apart.
            var apiBaseUrl = builder.Configuration["GildeApi:BaseUrl"]
                             ?? throw new InvalidOperationException(
                                 "GildeApi:BaseUrl is not set in appsettings.json.");

            builder.Services.AddHttpClient<IGildeApiClient, GildeApiClient>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            // The board page opens a SignalR connection straight to the API, so the
            // browser needs the API's address too.
            builder.Services.AddSingleton(new ApiEndpoints(apiBaseUrl));

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }

    /// <summary>Where the API lives, for the bits of the page that call it directly.</summary>
    public record ApiEndpoints(string BaseUrl)
    {
        public string BoardHubUrl => $"{BaseUrl.TrimEnd('/')}/hubs/board";
    }
}
