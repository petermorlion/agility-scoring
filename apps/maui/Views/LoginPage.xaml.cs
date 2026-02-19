using AgilityScoring.Maui.ViewModels;

namespace AgilityScoring.Maui.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}