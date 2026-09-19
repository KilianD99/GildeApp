using System.Windows.Input;
using GildeApp.Mobile.Services;

namespace GildeApp.Mobile.ViewModels
{
    public class JudgeSetupViewModel : BaseViewModel
    {
        private readonly JudgeIdentity _judge;
        private string _name = string.Empty;

        public JudgeSetupViewModel(JudgeIdentity judge)
        {
            _judge = judge;
            _name = judge.Name ?? string.Empty;

            SaveCommand = new Command(async () => await SaveAsync());
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public ICommand SaveCommand { get; }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = "Type the name the other judges will recognise.";
                return;
            }

            ClearError();
            _judge.Name = Name;

            await Shell.Current.GoToAsync("..");
        }
    }
}