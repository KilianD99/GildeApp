using GildeApp.Mobile.Services;
using GildeApp.Mobile.ViewModels;
using GildeApp.Mobile.Views;
using Mde.GildeApp.Mobile;
using Microsoft.Extensions.Logging;

namespace GildeApp.Mobile
{
    public static class MauiProgram
    {
        /// <summary>
        /// Where the API lives, as seen from the device.
        ///
        /// 10.0.2.2 is the Android emulator's alias for the host machine's localhost,
        /// so this reaches the API running in Visual Studio. Port 5069 is the API's
        /// HTTP endpoint from its launchSettings.json -- plain HTTP because the
        /// emulator does not trust the local dev certificate.
        ///
        /// Testing on a real phone instead: use the PC's LAN address
        /// (http://192.168.x.x:5069), add that address to
        /// Platforms/Android/Resources/xml/network_security_config.xml, and make the
        /// API listen beyond localhost with
        ///     dotnet run --urls http://0.0.0.0:5069
        /// </summary>
        public const string ApiBaseUrl = "http://10.0.2.2:5069/";

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // No ConfigureFonts call: the app uses the system font, so there is no
            // .ttf to ship. Add one back with .ConfigureFonts(...) plus a MauiFont
            // item in the csproj if you want a custom typeface.
            builder.UseMauiApp<App>();

            builder.Services.AddSingleton<JudgeIdentity>();

            builder.Services.AddHttpClient<IGildeApi, GildeApi>(client =>
            {
                client.BaseAddress = new Uri(ApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            builder.Services.AddSingleton<TourneysViewModel>();
            builder.Services.AddSingleton<TourneysPage>();

            // Transient: these two carry per-match state, so each navigation gets a
            // fresh one rather than the previous match's scores.
            builder.Services.AddTransient<MatchesViewModel>();
            builder.Services.AddTransient<MatchesPage>();
            builder.Services.AddTransient<ScoreViewModel>();
            builder.Services.AddTransient<ScorePage>();

            builder.Services.AddTransient<JudgeSetupViewModel>();
            builder.Services.AddTransient<JudgeSetupPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}