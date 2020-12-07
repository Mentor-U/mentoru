using MentorU.ViewModels;
using MentorU.Views;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace MentorU
{
    public partial class AppShell : Xamarin.Forms.Shell
    {

        AuthenticationResult authenticationResult;

        public AppShell(AuthenticationResult result)
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(EditProfilePage), typeof(EditProfilePage));
            Routing.RegisterRoute(nameof(MainChatPage), typeof(MainChatPage));
            Routing.RegisterRoute(nameof(ViewOnlyProfilePage), typeof(ViewOnlyProfilePage));

            authenticationResult = result;
        }

        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            IEnumerable<IAccount> accounts = await App.AuthenticationClient.GetAccountsAsync();

            while (accounts.Any())
            {
                await App.AuthenticationClient.RemoveAsync(accounts.First());
                accounts = await App.AuthenticationClient.GetAccountsAsync();
            }


            // not sure the right way to return to the login page... push a login on and close menu for now.
            // Eventually, I think its best to embed the logout and login both into the shell
            // We would need to "block" unoutherized users from looking at pages in the flyout this way
            // This should be easy to determine using the authenticatyionResult.AccessToken or something
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.Navigation.PushAsync(new LoginPage());
            Shell.Current.FlyoutIsPresented = false;   //close the menu 
        }


    }
}
