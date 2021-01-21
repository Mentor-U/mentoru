using MentorU.Services.Identity;
using Splat;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {

        public LoginPage()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Sends request out to MSAL to login/sign up 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            await Locator.Current.GetService<IIdentityService>().VerifyRegistration();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }


    }
}
