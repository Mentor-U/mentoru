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
        public ChatPage(Users ChatRecipient=null)
        {
            InitializeComponent();
            if (ChatRecipient != null)
                BindingContext = _vm = new ChatViewModel(ChatRecipient);
            else
                BindingContext = _vm = App.assistU.StartChat();

            ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Refresh",
                Command = _vm.RefreshChatCommand
            });
            _vm._messageListView = MessageListView;
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
