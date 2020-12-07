using MentorU.ViewModels;
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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                // Look for existing account
                IEnumerable<IAccount> accounts = await App.AuthenticationClient.GetAccountsAsync();

                AuthenticationResult result = await App.AuthenticationClient
                    .AcquireTokenSilent(Constants.Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
                if (result != null)
                {
                    if (result.Account.Username != "unknown")
                    {
                         Application.Current.MainPage = new AppShell(result);
                    }
                }
                       
            }
            catch
            {
                // Do nothing - the user isn't logged in
            }
            base.OnAppearing();
        }

        /// <summary>
        /// Sends request out to MSAL to login/sign up 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            AuthenticationResult result;
            try
            {
                // prompt the user to log in or sign up
                result = await App.AuthenticationClient
                    .AcquireTokenInteractive(Constants.Scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .WithParentActivityOrWindow(App.UIParent)
                    .ExecuteAsync();

                if (result != null)
                {
                    if (result.Account.Username != "unknown")
                    {
                        // user is authenticated, their informations stored in result and passed to the app shell for general availability
                        Application.Current.MainPage = new AppShell(result);
                    }
                }
            }
            catch (MsalException ex)
            {
                if (ex.Message != null && ex.Message.Contains("AADB2C90118"))
                {
                    result = await OnForgotPassword();
                    await Navigation.PushAsync(new LogoutPage(result));
                }
                else if (ex.ErrorCode != "authentication_canceled")
                {
                    await DisplayAlert("An error has occurred", "Exception message: " + ex.Message, "Dismiss");
                }
            }
        }

        /// <summary>
        /// Sends request out to MSAL to reset password
        /// </summary>
        /// <returns></returns>
        async Task<AuthenticationResult> OnForgotPassword()
        {
            try
            {
                return await App.AuthenticationClient
                    .AcquireTokenInteractive(Constants.Scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .WithParentActivityOrWindow(App.UIParent)
                    .WithB2CAuthority(Constants.AuthorityPasswordReset)
                    .ExecuteAsync();
            }
            catch (MsalException)
            {
                // Do nothing - ErrorCode will be displayed in OnLoginButtonClicked
                return null;
            }
        }
    }
}