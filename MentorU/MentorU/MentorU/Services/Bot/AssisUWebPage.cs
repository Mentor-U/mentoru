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
                Source = "https://assistu.azurewebsites.net/embed/"
                //Source = "https://portal.azure.com/#@mentoruauth.onmicrosoft.com/resource/subscriptions/acffcfa7-04dd-4570-a431-3b424ea362f9/resourceGroups/MentorU/providers/Microsoft.BotService/botServices/AssistU/test"
            };
            this.Content = browser;
        }
    }
}

