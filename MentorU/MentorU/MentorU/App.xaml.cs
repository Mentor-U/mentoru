using Azure.Storage.Blobs;
using MentorU.Models;
using MentorU.Services;
using MentorU.Services.Blob;
using MentorU.Services.DatabaseServices;
using MentorU.Services.LogOn;
using Xamarin.Forms;
using Microsoft.Extensions.Caching.Memory;
using Xamarin.Essentials;
using MentorU.Services.Bot;


namespace MentorU
{
    public partial class App : Application
    {
        // Hosted server for in app messaging
        public static string SignalRBackendUrl = "https://mentorusignalrserver.azurewebsites.net/messages";

        // local host testing:
        //public static string SignalRBackendUrl =
        //    DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:60089/messages" : "https://localhost:60089/messages";

        public static UserContext AADUser { get; internal set; }

        public static Users loggedUser { get; internal set; }

        // FIXME: Pull this lad from the DB
        public static AssistU assistU = new AssistU();

        public static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions() { });
        
        public App()
        {
            InitializeComponent();
            InitializeServices();

            Current.UserAppTheme = Current.RequestedTheme;

            // Callback if theme is changed during use of application
            Current.RequestedThemeChanged += (s, a) =>
            {
                Current.UserAppTheme = Current.RequestedTheme;
            };

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
            DependencyService.Register<DatabaseService>();
            DependencyService.Register<BlobService>();
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
