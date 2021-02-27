using MentorU.Models;
using MentorU.Services.DatabaseServices;
using MentorU.Services.LogOn;
using MentorU.Views;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    class LoginViewModel : BaseViewModel
    {

        public LoginViewModel()
        {
        }

        /// <summary>
        /// Sends request out to MSAL to login/sign up 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async Task<bool> OnLoginButtonClicked()
        {
            try
            {
                var userContext = await B2CAuthenticationService.Instance.SignInInteractively();
                App.AADUser = userContext;

                Users tempUser = new Users
                {
                    id = userContext.UserIdentifier,
                    FirstName = userContext.GivenName,
                    LastName = userContext.FamilyName,
                    DisplayName = userContext.Name,
                    Email = userContext.EmailAddress,
                };

                App.loggedUser = tempUser;

                bool isNew = await DatabaseService.Instance.tryCreateAccount(tempUser);

                if (isNew)
                {
                    await Application.Current.MainPage.Navigation.PushModalAsync(new NewProfileView());
                }
                else
                {
                    await Shell.Current.GoToAsync("///Home");
                }
                return true;

            }
            catch (Exception ex)
            {
                // Checking the exception message 
                // should ONLY be done for B2C
                // reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                {

                }
                    //OnPasswordReset();
                // Alert if any exception excluding user canceling sign-in dialog
                else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await AppShell.Current.DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
            return false;

        }

    }
}
