using MentorU.Services;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Microsoft.Identity.Client;
using Splat;
using MentorU.Services.Identity;
using MentorU.Models;

namespace MentorU
{
    public partial class App : Application
    {
        // For Databases
        public static MobileServiceClient client = new MobileServiceClient("https://mentoruapp.azurewebsites.net");

        // Hosted server for in app messaging
        public static string SignalRBackendUrl = "https://mentoruchat.azurewebsites.net/messages";
        // local host testing -> DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:60089" : "https://localhost:60089";


        // For MS Authentication 
        public static IPublicClientApplication PCA;
        public static object UIParent { get; set; } = null;

        // Keeps track of login status, helps know what pages to be displaying, etc.
        public static bool userSignedIn;
        public static Users ActiveUser;


        public App()
        {
            InitializeComponent();
            InitializeServices();

            ActiveUser = new Users();
            userSignedIn = false;

            MainPage = new AppShell();
        }

        /// <summary>
        ///   Initializes our Identity and Shell Routing services.
        ///   This is to have a consistant reference across the app to the same service.
        ///   Its a very similar pattern to the orinigal IDataStore Interface that uses the MockDataStore.cs
        /// </summary>
        private void InitializeServices()
        {
            // Services
            PCA = PublicClientApplicationBuilder.Create(Constants.ClientId)
             .WithRedirectUri($"msal{Constants.ClientId}://auth")
             .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
             .Build();

            Locator.CurrentMutable.RegisterLazySingleton<IIdentityService>(() => new IdentityServiceStub());

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
