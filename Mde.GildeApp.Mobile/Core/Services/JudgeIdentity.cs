namespace GildeApp.Mobile.Services
{
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