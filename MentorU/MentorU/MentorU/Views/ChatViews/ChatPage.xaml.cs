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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.OnAppearing();
        }
    }
}
