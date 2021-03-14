using System;

using Xamarin.Forms;

namespace MentorU.Services.Bot
{
    public class AssisUWebPage : ContentPage
    {
        public AssisUWebPage()
        {
            var browser = new WebView()
            {
                Source = "https://assistu.azurewebsites.net"
            };
            this.Content = browser;
        }
    }
}

