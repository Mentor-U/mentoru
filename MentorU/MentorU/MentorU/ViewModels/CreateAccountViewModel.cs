using MentorU.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class CreateAccountViewModel : BaseViewModel
    {

        private string _username;
        private string _email;
        private string _password;
        private string _confirmPassword;

        public Command OnCreateAccountClicked { get; }


        public CreateAccountViewModel()
        {
            OnCreateAccountClicked = new Command(CreateAccount);
            this.PropertyChanged +=
                (_, __) => OnCreateAccountClicked.ChangeCanExecute();
        }

        public string UserName
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        private async void CreateAccount()
        {
            if (Password == ConfirmPassword)
            {
                Users newProfile = new Users()
                {
                    Email = Email,
                    Password = Password,
                };

                await App.client.GetTable<Users>().InsertAsync(newProfile);
                await Application.Current.MainPage.DisplayAlert("Success", "Account Created", "Ok");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Account NOT Created", "Ok");
            }
        }

    }
}
