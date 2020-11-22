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
        }

        private void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
            Application.Current.MainPage = new NavigationPage(new LoginPage());
            //await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
