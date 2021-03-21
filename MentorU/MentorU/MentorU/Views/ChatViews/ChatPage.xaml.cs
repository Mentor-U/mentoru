using System;
using System.Collections.Generic;
using MentorU.ViewModels;
using MentorU.Models;
using Xamarin.Forms;

namespace MentorU.Views.ChatViews
{
    public partial class ChatPage : ContentPage
    {
        ChatViewModel _vm;
        public ChatPage(Users ChatRecipient)
        {
            InitializeComponent();
            BindingContext = _vm = new ChatViewModel(ChatRecipient);

            ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Refresh",
                Command = _vm.RefreshChatCommand
            });

            WebView browser = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.OnAppearing();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _ = _vm.Disconnect();
        }
    }
}
