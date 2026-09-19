using Android.App;
using Android.Content.PM;
using Android.OS;

// Same namespace as the rest of the app -- see the note in MainApplication.cs.
namespace GildeApp.Mobile
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize
                               | ConfigChanges.Orientation
                               | ConfigChanges.UiMode
                               | ConfigChanges.ScreenLayout
                               | ConfigChanges.SmallestScreenSize
                               | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // A judge scoring a bout should not have the screen time out mid-match.
            Window?.AddFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
        }
    }
}
