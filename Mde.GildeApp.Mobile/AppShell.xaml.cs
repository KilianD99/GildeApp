using GildeApp.Mobile.Views;

namespace GildeApp.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

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