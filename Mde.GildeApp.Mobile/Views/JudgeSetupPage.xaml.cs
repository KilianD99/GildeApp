using GildeApp.Mobile.ViewModels;

namespace GildeApp.Mobile.Views
{
    public partial class JudgeSetupPage : ContentPage
    {
        public JudgeSetupPage(JudgeSetupViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}