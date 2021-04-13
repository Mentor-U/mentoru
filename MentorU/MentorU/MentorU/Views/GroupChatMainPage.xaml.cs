using MentorU.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views.ChatViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroupChatMainPage : ContentPage
    {
        private GroupMainChatViewModel _viewModel;
        public GroupChatMainPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new GroupMainChatViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

      
    }
}