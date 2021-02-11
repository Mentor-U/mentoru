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
            BindingContext = _viewModel = new LoadingViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.Init();
		}
    }
}