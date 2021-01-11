using MentorU.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class CreateAccountViewModel : BaseViewModel
    {

        private string _firstname;
        private string _lastname;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _major;
        private string _bio;
        private string _role;
        private ObservableCollection<string> _classes;

        public Command OnCreateAccountClicked { get; }
        public Command AddClassClicked { get; }


        public CreateAccountViewModel()
        {
            OnCreateAccountClicked = new Command(CreateAccount);
            AddClassClicked = new Command(AddClass);
            _classes = new ObservableCollection<string>();
            this.PropertyChanged +=
                (_, __) => OnCreateAccountClicked.ChangeCanExecute();
        }

        public string FirstName
        {
            get => _firstname;
            set => SetProperty(ref _firstname, value);
        }

        public string LastName
        {
            get => _lastname;
            set => SetProperty(ref _lastname, value);
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

        public string Major
        {
            get => _major;
            set => SetProperty(ref _major, value);
        }

        public string Bio
        {
            get => _bio;
            set => SetProperty(ref _bio, value);
        }

        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        private async void CreateAccount()
        {
            if (Password == ConfirmPassword)
            {
                Users newProfile = new Users()
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Password = Password,
                    Major = Major,
                    Bio = Bio,
                    Role = Role,
                };
                await App.client.GetTable<Users>().InsertAsync(newProfile);
                await Application.Current.MainPage.DisplayAlert("Success", "Account Created", "Ok");

            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Account NOT Created", "Ok");
            }
        }

        private async void AddClass()
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync("Add a class", "E.g, CS2420 Data Structure");
            if(!String.IsNullOrWhiteSpace(result))
            {
                _classes.Add(result);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Invalid Class!", "Ok");
            }
        }

    }
}
