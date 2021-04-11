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
                Source = "https://webchat.botframework.com/embed/AssistU?s=20LzcCzdXBg.IrynKUwXT74ePL49AUQhCjtdS4IH9XX1XJmvGjJaYjc",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            this.Content = browser;
        }
    }
}

