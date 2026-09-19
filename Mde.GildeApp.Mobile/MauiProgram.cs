// IGildeApi lives in Core/Services/Interfaces, JudgeIdentity and GildeApi in
// Core/Services but under the shorter GildeApp.Mobile.Services namespace -- both
// have to be imported here or AddHttpClient<IGildeApi, GildeApi> will not compile.
using GildeApp.Mobile.Core.Services.Interfaces;
using GildeApp.Mobile.Services;
using GildeApp.Mobile.ViewModels;
using GildeApp.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace GildeApp.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // No ConfigureFonts call: the app uses the system font, so there is no
            // .ttf to ship. Add one back with .ConfigureFonts(...) plus a MauiFont
            // item in the csproj if you want a custom typeface.
            builder.UseMauiApp<App>();

            builder.Services.AddSingleton<JudgeIdentity>();
            builder.Services.AddSingleton<ServerSettings>();

            // No BaseAddress here on purpose. The server address is a setting the
            // judge can change on the setup screen, and BaseAddress is fixed when the
            // client is built at startup -- so GildeApi builds an absolute URI per
            // request from ServerSettings instead.
            builder.Services.AddHttpClient<IGildeApi, GildeApi>(client =>
            {
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