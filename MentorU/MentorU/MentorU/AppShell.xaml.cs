using MentorU.Services.LogOn;
using MentorU.Views;
using System;
using Xamarin.Forms;

namespace MentorU
{
    public partial class AppShell : Xamarin.Forms.Shell
    {

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(EditProfilePage), typeof(EditProfilePage));
            Routing.RegisterRoute(nameof(MainChatPage), typeof(MainChatPage));
            Routing.RegisterRoute(nameof(ViewOnlyProfilePage), typeof(ViewOnlyProfilePage));
            Routing.RegisterRoute("Main/Login", typeof(LoginPage));
            Routing.RegisterRoute(nameof(NotificationPage), typeof(NotificationPage));
        }

        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;   //close the menu 

            var userContext = await B2CAuthenticationService.Instance.SignOutAsync();
            App.AADUser = userContext;
           
            await GoToAsync("///Login");
        }
    }
}
