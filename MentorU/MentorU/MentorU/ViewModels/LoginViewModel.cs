using MentorU.Models;
using MentorU.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
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

        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }

        private async void OnLoginClicked(object obj)
        {
            try
            {
                //string email = Email;
                var user = await App.client.GetTable<Users>().Where(e => e.Email == Email).ToListAsync();
                bool isPasswordMatched = false;
                foreach (Users u in user)
                {
                    isPasswordMatched = VerifyPassword(Password, u.Hash, u.Salt);
                }

                if(isPasswordMatched)
                {
                    //var userList = await App.client.GetTable<Users>().Where(u => u.Password == Password).ToListAsync();
                    foreach (Users logged in user)
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

                //var pwd = await App.client.GetTable<Users>().Where(e => e.Email == email)
                //.Select(p => p.Password).ToListAsync();

                //if (pwd.Contains(Password))
                //{
                //    var userList = await App.client.GetTable<Users>().Where(user => user.Password == Password).ToListAsync();
                //    foreach(Users logged in userList)
                //    {
                //        App.loggedUser = logged;
                //    }

                //    Application.Current.MainPage = new AppShell();
                //    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                //}
                //else
                //{
                //    await Application.Current.MainPage.DisplayAlert("Failed", "Email or Password Incorrect!", "Ok");
                //}
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Email or Password Incorrect!", "Ok");
            }
        }

        private async void OnCreateClicked(object obj)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new CreateAccountPage());
        }
    }
}
