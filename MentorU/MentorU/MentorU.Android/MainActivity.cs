﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.Identity.Client;
using Android.Content;
using Xamarin.Forms;
using MentorU.Services.LogOn;
using Plugin.CurrentActivity;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using ImageCircle.Forms.Plugin.Droid;
using System;
using Android.Runtime;
using Firebase.Iid;
using MentorU.Droid.Services;
using MentorU.Services;
using Firebase.Installations;
using Firebase;
using Firebase.Provider;

namespace MentorU.Droid
{
    [Activity(Label = "MentorU", LaunchMode = LaunchMode.SingleTop, Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, Android.Gms.Tasks.IOnSuccessListener
    {
        IPushNotificationActionService _notificationActionService;
        IDeviceInstallationService _deviceInstallationService;

        IPushNotificationActionService NotificationActionService
            => _notificationActionService ??
                (_notificationActionService =
                ServiceContainer.Resolve<IPushNotificationActionService>());

        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService =
                ServiceContainer.Resolve<IDeviceInstallationService>());

        internal static MainActivity Instance { get; private set; }
        
        //FirebaseInitProvider fireBase { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            DependencyService.Register<IParentWindowLocatorService, AndroidParentWindowLocatorService>();

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //fireBase = new FirebaseInitProvider();
            

            Bootstrap.Begin(() => new DeviceInstallationService());

            if (DeviceInstallationService.NotificationsSupported)
            {
                FirebaseInstanceId.GetInstance(Firebase.FirebaseApp.Instance)
                    .GetInstanceId()
                    .AddOnSuccessListener(this);
            }

            //if (DeviceInstallationService.NotificationsSupported)
            //{
            //    FirebaseInstallations.GetInstance(Firebase.FirebaseApp.Instance).GetId()
            //        .AddOnSuccessListener(this);
            //}

            Rg.Plugins.Popup.Popup.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
     
            CurrentPlatform.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

            ImageCircleRenderer.Init();

            //FirebaseApp.InitializeApp(this.ApplicationContext);

            LoadApplication(new App());

            ProcessNotificationActions(Intent);

            // Keyboard visibility adjustment
            App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        // Field, property, and method for Picture Picker
        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        /// <summary>
        /// Redirections for auth on android
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
           
            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (data != null))
                {
                    Android.Net.Uri uri = data.Data;
                    Stream stream = ContentResolver.OpenInputStream(uri);

                    // Set the Stream as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(stream);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
            else
            {
                AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
            }
        }


        /// <summary>
        /// Overriding the back button on Android to handle the custom popup
        /// interactions correctly.
        /// </summary>
        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
            }
            else
                App.Current.MainPage.Navigation.PopToRootAsync();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            ProcessNotificationActions(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnSuccess(Java.Lang.Object result)
            => DeviceInstallationService.Token =
                result.Class.GetMethod("getToken").Invoke(result).ToString();

        void ProcessNotificationActions(Intent intent)
        {
            try
            {
                if (intent?.HasExtra("action") == true)
                {
                    var action = intent.GetStringExtra("action");

                    if (!string.IsNullOrEmpty(action))
                        NotificationActionService.TriggerAction(action);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
