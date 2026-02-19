using AgilityScoring.Maui.ViewModels;

namespace AgilityScoring.Maui.Views
{
    public partial class IndexPage : ContentPage
    {
        public IndexPage(IndexViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is IndexViewModel viewModel)
            {
                await viewModel.InitializeAsync();
            }
        }
    }
}