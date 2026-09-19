using GildeApp.Mobile.Views;

namespace GildeApp.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Pages reached by navigation rather than from the shell's own tabs have
            // to be registered by route before GoToAsync can find them.
            Routing.RegisterRoute(Routes.Matches, typeof(MatchesPage));
            Routing.RegisterRoute(Routes.Score, typeof(ScorePage));
            Routing.RegisterRoute(Routes.JudgeSetup, typeof(JudgeSetupPage));
        }
    }

    public static class Routes
    {
        public const string Matches = "matches";
        public const string Score = "score";
        public const string JudgeSetup = "judge";
    }
}
