using MentorU.Models;
using MentorU.Views;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        public Command CreateCommand { get; }

        private string _email;
        private string _password;

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

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
            CreateCommand = new Command(OnCreateClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            //await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            //await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");

            try
            {
                string email = Email;
                //string password = Password;

                var pwd = await App.client.GetTable<Users>().Where(e => e.Email == email)
                    .Select(p => p.Password).ToListAsync();

                if (pwd.Contains(Password))
                {
                    Application.Current.MainPage = new AppShell();
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Failed", "Email or Password Incorrect!", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Email or Password Incorrect!", "Ok");
            }
            

            //SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation);
            //conn.CreateTable<Users>();
            //var profiles = conn.Table<Users>().ToList();
            //conn.Close();

            //bool loginSuccess = false;
            //foreach(Users P in profiles)
            //{
            //    if(P.Email.Equals(Email) && P.Password.Equals(Password))
            //    {
            //        loginSuccess = true;
            //    }
            //}

            //if(loginSuccess)
            //{
            //    Application.Current.MainPage = new AppShell();
            //    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            //}
            //else
            //{
            //    await Application.Current.MainPage.DisplayAlert("Failed", "Account NOT Created", "Ok");

            //}

        }

        private async void OnCreateClicked(object obj)
        {
            //CreateAccountViewModel _viewModel = new CreateAccountViewModel();
            //CreateAccount createAccount = new CreateAccount();
            //createAccount.BindingContext = _viewModel;
            await Application.Current.MainPage.Navigation.PushModalAsync(new CreateAccount());
        }
    }
}
