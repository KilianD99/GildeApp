using System.Windows.Input;
using GildeApp.Mobile.Services;

namespace GildeApp.Mobile.ViewModels
{
    public class JudgeSetupViewModel : BaseViewModel
    {
        private readonly JudgeIdentity _judge;
        private readonly ServerSettings _server;

        private string _name = string.Empty;
        private string _serverUrl = string.Empty;

        public JudgeSetupViewModel(JudgeIdentity judge, ServerSettings server)
        {
            _judge = judge;
            _server = server;

            _name = judge.Name ?? string.Empty;
            _serverUrl = server.BaseUrl;

            SaveCommand = new Command(async () => await SaveAsync());
            UseEmulatorAddressCommand = new Command(() => ServerUrl = ServerSettings.EmulatorDefault);
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string ServerUrl
        {
            get => _serverUrl;
            set => Set(ref _serverUrl, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand UseEmulatorAddressCommand { get; }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = "Type the name the other judges will recognise.";
                return;
            }

            if (!ServerSettings.IsValid(ServerUrl))
            {
                ErrorMessage = "That server address will not work. Something like " +
                               "192.168.1.20:5069 or http://10.0.2.2:5069";
                return;
            }

            ClearError();

            _judge.Name = Name;
            _server.BaseUrl = ServerUrl;

            // Show what it was tidied into, so a typed "192.168.1.20:5069" visibly
            // becomes "http://192.168.1.20:5069/" rather than looking ignored.
            ServerUrl = _server.BaseUrl;

            await Shell.Current.GoToAsync("..");
        }
    }
}
