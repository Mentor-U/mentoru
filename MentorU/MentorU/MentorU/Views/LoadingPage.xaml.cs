using MentorU.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage
    {
        LoadingViewModel _viewModel;
        public LoadingPage()
        {
            InitializeComponent();
            _viewModel = new LoadingViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.TrySignIn();
		}
    }
}