using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MentorU.ViewModels;
using MentorU.Services;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        HomeViewModel _vm;
        readonly INotificationRegistrationService _notificationRegistrationService;

        public HomePage()
        {
            InitializeComponent();
            _notificationRegistrationService =
                ServiceContainer.Resolve<INotificationRegistrationService>();
            _notificationRegistrationService.RegisterDeviceAsync();
            BindingContext = _vm = new HomeViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.OnAppearing();
        }
            
    }
}