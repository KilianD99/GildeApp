namespace GildeApp.Mobile.Services
{
    /// <summary>
    /// Where the scoring API lives, as typed by whoever set the phone up.
    ///
    /// This is a setting rather than a constant because the right address depends on
    /// where the app is running: the emulator reaches the dev machine at 10.0.2.2, a
    /// phone on the club wifi needs the PC's LAN address, and a router hands out a
    /// different lease every so often. Baking one into the source means recompiling
    /// every time any of that changes.
    /// </summary>
    public class ServerSettings
    {
        private const string Key = "server.baseurl";

        /// <summary>The Android emulator's alias for the host machine's localhost.</summary>
        public const string EmulatorDefault = "http://10.0.2.2:5069/";

        public string BaseUrl
        {
            get
            {
                var stored = Preferences.Default.Get(Key, string.Empty);
                return string.IsNullOrWhiteSpace(stored) ? EmulatorDefault : stored;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    Preferences.Default.Remove(Key);
                else
                    Preferences.Default.Set(Key, Normalise(value));
            }
        }

        /// <summary>
        /// Requests are built against this. Read fresh each time, so changing the
        /// address takes effect on the next call instead of the next launch.
        /// </summary>
        public Uri BaseUri => new(BaseUrl);

        /// <summary>
        /// Accepts what someone would actually type -- "192.168.1.20:5069" -- and
        /// fills in the bits they left out.
        /// </summary>
        public static string Normalise(string value)
        {
            var text = value.Trim();

            if (!text.Contains("://", StringComparison.Ordinal))
                text = "http://" + text;

            if (!text.EndsWith('/'))
                text += "/";

            return text;
        }

        /// <summary>True when the text parses as an absolute http(s) address.</summary>
        public static bool IsValid(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return Uri.TryCreate(Normalise(value), UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
