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
    }
}