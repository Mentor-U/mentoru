using MentorU.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class CreateAccountViewModel : BaseViewModel
    {
        public Command CreateAccountCommand { get; }

        public CreateAccountViewModel()
        {
            CreateAccountCommand = new Command(OnCreateAccountClicked);
        }

        private async void OnCreateAccountClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            //await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            //await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
            Shell.Current.SendBackButtonPressed();
        }

    }
}
