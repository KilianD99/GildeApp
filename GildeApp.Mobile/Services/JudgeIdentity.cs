namespace GildeApp.Mobile.Services
{
    /// <summary>
    /// Who is holding this phone. Typed once and kept in the device's preferences,
    /// so the judge is not asked again on every launch.
    ///
    /// This is deliberately not authentication -- nothing stops someone typing
    /// another judge's name. It exists so a claimed match can say "Marie is scoring
    /// this" instead of just "taken". Replace it with a real token when the API
    /// grows accounts.
    /// </summary>
    public class JudgeIdentity
    {
        private const string Key = "judge.name";

        public string? Name
        {
            get
            {
                var stored = Preferences.Default.Get(Key, string.Empty);
                return string.IsNullOrWhiteSpace(stored) ? null : stored;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    Preferences.Default.Remove(Key);
                else
                    Preferences.Default.Set(Key, value.Trim());
            }
        }

        public bool IsSet => Name is not null;
    }
}
