using MentorU.Services.LogOn;
using Microsoft.Identity.Client;
using System;
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

        /// <summary>
        /// Sends request out to MSAL to login/sign up 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var userContext = await B2CAuthenticationService.Instance.SignInAsync();
                App.AADUser = userContext;
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");

            }
            catch (Exception ex)
            {
                // Checking the exception message 
                // should ONLY be done for B2C
                // reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                    OnPasswordReset();
                // Alert if any exception excluding user canceling sign-in dialog
                else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }

        }

        private void OnPasswordReset()
        {
            throw new NotImplementedException();

            //try
            //{
            //    var userContext = await B2CAuthenticationService.Instance.ResetPasswordAsync();
            //}
            //catch (Exception ex)
            //{
            //    // Alert if any exception excluding user canceling sign-in dialog
            //    if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
            //        await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            //}
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
