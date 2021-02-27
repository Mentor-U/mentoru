using MentorU.Models;
using MentorU.Services.DatabaseServices;
using MentorU.Services.LogOn;
using MentorU.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    class LoadingViewModel : BaseViewModel
    {

        public LoadingViewModel()
        {

        }

        // Called by the views OnAppearing method
        public async Task TrySignIn()
        {
            try
            {
                var userContext = await B2CAuthenticationService.Instance.SignInAsync();
                App.AADUser = userContext;

                Users tempUser = new Users
                {
                    id = userContext.UserIdentifier,
                    FirstName = userContext.GivenName,
                    LastName = userContext.FamilyName,
                    DisplayName = userContext.Name,
                    Email = userContext.EmailAddress
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

            }
            catch (Exception ex)
            {
                await Shell.Current.GoToAsync("///Login");
            }
            //var userContext = await B2CAuthenticationService.Instance.SignInAsync();
           
        }
    }
}
