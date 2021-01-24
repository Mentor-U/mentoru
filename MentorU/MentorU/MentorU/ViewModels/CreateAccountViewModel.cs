using MentorU.Models;
using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
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

        //private static string getHash(string password)
        //{
        //    using (var sha256 = SHA256.Create())
        //    {
        //        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        //    }
        //}

        //private static string getSalt()
        //{
        //    byte[] bytes = new byte[128 / 8];
        //    using (var keyGenerator = RandomNumberGenerator.Create())
        //    {
        //        keyGenerator.GetBytes(bytes);
        //        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        //    }
        //}

        public class HashSalt
        {
            public string Hash { get; set; }
            public string Salt { get; set; }
        }

        public static HashSalt GenerateSaltedHash(int size, string password)
        {
            var saltBytes = new byte[size];
            var provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            HashSalt hashSalt = new HashSalt { Hash = hashPassword, Salt = salt };
            return hashSalt;
        }

        private async void CreateAccount()
        {
            if (Password == ConfirmPassword)
            {
                HashSalt hashSalt = GenerateSaltedHash(64, Password);
                Users newProfile = new Users()
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Password = Password,
                    Major = Major,
                    Bio = Bio,
                    Role = Role,
                    Hash = hashSalt.Hash,
                    Salt = hashSalt.Salt
                   
                };
                await App.client.GetTable<Users>().InsertAsync(newProfile);
                await Application.Current.MainPage.DisplayAlert("Success", "Account Created", "Ok");
            }
            else if(Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Password and Confirm Password do not match", "Ok");
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
