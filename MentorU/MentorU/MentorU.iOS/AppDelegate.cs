using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Microsoft.Identity.Client;
using Microsoft.WindowsAzure.MobileServices;
using UIKit;
using Xamarin;

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
            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init();
           
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
            return base.OpenUrl(app, url, options);
        }

    }


}
