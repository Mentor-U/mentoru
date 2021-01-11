using MentorU.ViewModels;
using MentorU.Views;
using System;
using System.Collections.Generic;
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
        }

        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
