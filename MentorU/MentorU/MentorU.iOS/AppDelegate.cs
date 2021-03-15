using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Microsoft.Identity.Client;
using Microsoft.WindowsAzure.MobileServices;
using UIKit;
using Xamarin;
using MentorU.iOS.Extensions;
using MentorU.iOS.Services;
using MentorU.Services;
using UserNotifications;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace MentorU.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //// create a new window instance based on the screen size
            //window = new UIWindow(UIScreen.MainScreen.Bounds);

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init();

            Bootstrap.Begin(() => new DeviceInstallationService());
            if (DeviceInstallationService.NotificationsSupported)
            {
                UNUserNotificationCenter.Current.RequestAuthorization(
                        UNAuthorizationOptions.Alert |
                        UNAuthorizationOptions.Badge |
                        UNAuthorizationOptions.Sound,
                        (approvalGranted, error) =>
                        {
                            if (approvalGranted && error == null)
                                RegisterForRemoteNotifications();
                        });
            }

            using (var userInfo = options?.ObjectForKey(
                    UIApplication.LaunchOptionsRemoteNotificationKey) as NSDictionary)
                                ProcessNotificationActions(userInfo);

            CurrentPlatform.Init();

            // Shifts UI elements to make room for the keyboard
            IQKeyboardManager.SharedManager.Enable = true;

            LoadApplication(new App());

            Rg.Plugins.Popup.Popup.Init();
            ImageCircleRenderer.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            //AppCenter.Start("fb0078a5-c257-43b7-b61d-735790293192",
            //       typeof(Analytics), typeof(Crashes));

            return base.FinishedLaunching(app, options);
        }


        /// <summary>
        /// Redirections for authentication on IOS
        /// </summary>
        /// <param name="app"></param>
        /// <param name="url"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
            return true;
        }


        //--- Notifications properties ---

        IPushNotificationActionService _notificationActionService;
        INotificationRegistrationService _notificationRegistrationService;
        IDeviceInstallationService _deviceInstallationService;

        IPushNotificationActionService NotificationActionService
            => _notificationActionService ??
                (_notificationActionService =
                ServiceContainer.Resolve<IPushNotificationActionService>());

        INotificationRegistrationService NotificationRegistrationService
            => _notificationRegistrationService ??
                (_notificationRegistrationService =
                ServiceContainer.Resolve<INotificationRegistrationService>());

        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService =
                ServiceContainer.Resolve<IDeviceInstallationService>());


        /// <summary>
        /// Register the device to the notification hub
        /// </summary>
        void RegisterForRemoteNotifications()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert |
                    UIUserNotificationType.Badge |
                    UIUserNotificationType.Sound,
                    new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            });
        }

        Task CompleteRegistrationAsync(NSData deviceToken)
        {
            DeviceInstallationService.Token = deviceToken.ToHexString();
            return NotificationRegistrationService.RefreshRegistrationAsync();
        }

        /// <summary>
        /// Process the notification to be displayed 
        /// </summary>
        /// <param name="userInfo"></param>
        void ProcessNotificationActions(NSDictionary userInfo)
        {
            if (userInfo == null)
                return;

            try
            {
                var actionValue = userInfo.ObjectForKey(new NSString("action")) as NSString;

                if (!string.IsNullOrWhiteSpace(actionValue?.Description))
                    NotificationActionService.TriggerAction(actionValue.Description);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        public override void RegisteredForRemoteNotifications(
            UIApplication application,
            NSData deviceToken)
            => CompleteRegistrationAsync(deviceToken).ContinueWith((task)
                => { if (task.IsFaulted) throw task.Exception; });


        public override void ReceivedRemoteNotification(
            UIApplication application,
            NSDictionary userInfo)
            => ProcessNotificationActions(userInfo);


        public override void FailedToRegisterForRemoteNotifications(
            UIApplication application,
            NSError error)
            => Debug.WriteLine(error.Description);

    }


}
