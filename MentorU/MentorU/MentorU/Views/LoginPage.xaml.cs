using MentorU.Models;
using MentorU.Services.DatabaseServices;
using MentorU.Services.LogOn;
using MentorU.ViewModels;
using Microsoft.Identity.Client;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        LoginViewModel _viewModel;
        public LoginPage()
        {
            NavigationPage.SetHasBackButton(this, false);
            InitializeComponent();
            BindingContext = _viewModel = new LoginViewModel();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void loginButton_Clicked(object sender, EventArgs e)
        {
           await _viewModel.OnLoginButtonClicked();
        }
    }
}
