using MentorU.Services;
using MentorU.Views;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MentorU.Models;

namespace MentorU
{
    public partial class App : Application
    {

        public static MobileServiceClient client = new MobileServiceClient("https://mentoruapp.azurewebsites.net");

        public static Users loggedUser; 

        public App()
        {
            InitializeComponent();


            DependencyService.Register<MockDataStore>();
            var isLoggedIn = Xamarin.Essentials.SecureStorage.GetAsync("isLogged").Result;
            if (isLoggedIn == "1")
            {
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new LoginPage();
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
