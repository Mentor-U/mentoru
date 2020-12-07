using MentorU.Models;
using MentorU.Views;
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
            try
            {
                string email = Email;

                var pwd = await App.client.GetTable<Users>().Where(e => e.Email == email)
                    .Select(p => p.Password).ToListAsync();

                if (pwd.Contains(Password))
                {
                    var userList = await App.client.GetTable<Users>().Where(user => user.Password == Password).ToListAsync();
                    foreach(Users logged in userList)
                    {
                        App.loggedUser = logged;
                    }

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
        }

        private async void OnCreateClicked(object obj)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new CreateAccount());
        }
    }
}
