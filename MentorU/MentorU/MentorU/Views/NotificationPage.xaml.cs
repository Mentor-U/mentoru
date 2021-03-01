using MentorU.ViewModels;
using Xamarin.Forms;


namespace MentorU.Views
{
    public partial class NotificationPage : ContentPage
    {
        NotificationViewModel vm;
        bool active = true;
        public NotificationPage()
        {
            InitializeComponent();
            BindingContext = vm = new NotificationViewModel();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.OnAppearing();
        }
    }
}
