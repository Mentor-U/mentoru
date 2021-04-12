using System;
using System.Collections.Generic;
using MentorU.ViewModels;
using MentorU.Models;
using Xamarin.Forms;

namespace MentorU.Views.ChatViews
{

    public partial class GroupChatPage : ContentPage
    {

        GroupChatViewModel _vm;
        public GroupChatPage(string groupName)
        {
            InitializeComponent();

            BindingContext = _vm = new GroupChatViewModel(groupName);


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