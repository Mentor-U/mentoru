using MentorU.Models;
using SQLite;
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
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            //await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            //await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
            if (Password == ConfirmPassword)
            {
                Users newProfile = new Users()
                {
                    Email = Email,
                    Password = Password,
                };


                //LOCAL DB
                //SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation);
                //conn.CreateTable<Users>();
                //int rows = conn.Insert(newProfile);
                //conn.Close();

                await App.client.GetTable<Users>().InsertAsync(newProfile);
                await Application.Current.MainPage.DisplayAlert("Success", "Account Created", "Ok");
                // If insert successful
                //if (rows > 0)
                //{
                //    await Application.Current.MainPage.DisplayAlert("Success", "Account Created", "Ok");
                //}
                //else
                //{
                //    await Application.Current.MainPage.DisplayAlert("Failed", "Account NOT Created", "Ok");
                //}
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Account NOT Created", "Ok");
            }

        }

    }
}
