using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    /// <summary>
    /// This logout is UNUSED for now with the MSAL stuff. Im doing the same actions using the logout method from appshell.
    /// Maybe we can embed this logout as a way for the user to confirm if they want to log out or not,
    /// as well as keep the whole app in the AppShell and not use the Navigation class for pages at all.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogoutPage : ContentPage
    {

        AuthenticationResult authenticationResult;

        public LogoutPage(AuthenticationResult result)
        {
            InitializeComponent();
            authenticationResult = result;
        }

        protected override void OnAppearing()
        {
            if (authenticationResult != null)
            {
                if (authenticationResult.Account.Username != "unknown")
                {
                    messageLabel.Text = string.Format("Welcome {0}", authenticationResult.Account.Username);
                }
                else
                {
                    messageLabel.Text = string.Format("UserId: {0}", authenticationResult.Account.Username);
                }
            }

            base.OnAppearing();
        }

        async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            IEnumerable<IAccount> accounts = await App.AuthenticationClient.GetAccountsAsync();

            while (accounts.Any())
            {
                await App.AuthenticationClient.RemoveAsync(accounts.First());
                accounts = await App.AuthenticationClient.GetAccountsAsync();
            }

            await Navigation.PopAsync();
        }

    }
}