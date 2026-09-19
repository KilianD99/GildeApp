using Android.App;
using Android.Runtime;

// Namespace deliberately GildeApp.Mobile, matching <RootNamespace> and the rest of
// the app -- not Mde.GildeApp.Mobile, which is what the project template generated
// from the project's file name.
//
// This is not cosmetic. MauiProgram lives in GildeApp.Mobile, so from inside
// Mde.GildeApp.Mobile the call below does not resolve. And adding
// "using GildeApp.Mobile;" there does NOT fix it: C# resolves the first segment by
// walking up the enclosing namespaces, finds Mde.GildeApp, and quietly imports
// Mde.GildeApp.Mobile -- this very namespace -- instead of the one you meant. Sharing
// the namespace sidesteps the whole trap.
namespace GildeApp.Mobile
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
