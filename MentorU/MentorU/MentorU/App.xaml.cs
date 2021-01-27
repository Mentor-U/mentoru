using MentorU.Services;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using MentorU.Services.LogOn;
using MentorU.Models;

namespace MentorU
{
    public partial class App : Application
    {
        // For Databases
        public static MobileServiceClient client = new MobileServiceClient("https://mentoruapp.azurewebsites.net");

        // Hosted server for in app messaging
        public static string SignalRBackendUrl = "https://mentoruchat.azurewebsites.net/messages";

        // local host testing:
        // DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:60089/messages" : "https://localhost:60089/messages";


        public static UserContext AADUser { get; internal set; }

        public static Users loggedUser { get; internal set; }

        // local host testing -> DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:60089" : "https://localhost:60089";

        public App()
        {
            InitializeComponent();
            InitializeServices();

            MainPage = new AppShell();
        }

        /// <summary>
        ///   Initializes our Identity and Shell Routing services.
        ///   This is to have a consistant reference across the app to the same service.
        ///   Its a very similar pattern to the orinigal IDataStore Interface that uses the MockDataStore.cs
        /// </summary>
        private void InitializeServices()
        {
            DependencyService.Register<B2CAuthenticationService>();
            DependencyService.Register<MockDataStore>();
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
