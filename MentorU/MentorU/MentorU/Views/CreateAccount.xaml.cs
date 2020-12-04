using MentorU.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MentorU.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAccount : ContentPage
    {
        CreateAccountViewModel _viewModel;
        readonly ListView listView;
        public CreateAccount()
        {
            InitializeComponent();
            BindingContext = _viewModel = new CreateAccountViewModel();
            this.BackgroundColor = Color.White;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            listView.ItemsSource = await App.Database.GetUserAsync();
        }

        private void OnCreateAccountClicked(object sender, EventArgs e)
        {
            if(password.Text == confirmPassword.Text)
            {
                Profile newProfile = new Profile()
                {
                    UserName = userName.Text,
                    Email = email.Text,
                    Password = password.Text,
                };

                SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation);
                conn.CreateTable<Profile>();
                int rows = conn.Insert(newProfile);
                conn.Close();


                if(rows > 0)
                {
                    DisplayAlert("Success", "Profile inserted", "Ok");
                }
                else
                {
                    DisplayAlert("Failed", "Profile not inserted", "Ok");
                }
            }
         
        }
    }
}