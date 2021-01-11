using MentorU.Services;
using MentorU.Views;
using Microsoft.Identity.Client;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MentorU.Models;
using Xamarin.Essentials;


namespace MentorU
{
    public partial class App : Application
    {

        public static MobileServiceClient client = new MobileServiceClient("https://mentoruapp.azurewebsites.net");

        //Hosted server for in app messaging
        public static string SignalRBackendUrl = "https://mentoruchat.azurewebsites.net/messages";
            // local host testing -> DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:60089" : "https://localhost:60089";

        public static Users loggedUser; 

        public App()
        {
            InitializeComponent();


            DependencyService.Register<MockDataStore>();

            // Set up the auth client for MSAL
            AuthenticationClient = PublicClientApplicationBuilder.Create(Constants.ClientId)
                .WithIosKeychainSecurityGroup(Constants.IosKeychainSecurityGroups)
                .WithB2CAuthority(Constants.AuthoritySignin)
                .WithRedirectUri($"msal{Constants.ClientId}://auth")
                .Build();

            // Send the user to the login page
            MainPage = new NavigationPage(new LoginPage());
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
