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
            Routing.RegisterRoute("Main/Login", typeof(LoginPage));
        }

        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;   //close the menu 
            await GoToAsync("Main/Login");
        }
    }
}
