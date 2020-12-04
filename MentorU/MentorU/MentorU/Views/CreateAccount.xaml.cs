using MentorU.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MentorU.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(email.Text) && !string.IsNullOrEmpty(password.Text))
            {
                await App.Database.SaveUserAsync(new Profile
                {
                    UserName = userName.Text,
                    Email = email.Text,
                    //Password = password.Text
                });

                email.Text = password.Text = string.Empty;
                listView.ItemsSource = await App.Database.GetUserAsync();
            }
        }
    }
}