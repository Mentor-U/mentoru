using MentorU.Models;
using MentorU.Views.ChatViews;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using MentorU.Services.DatabaseServices;
using MentorU.Services.Blob;
using Rg.Plugins.Popup.Services;
using MentorU.Views;

namespace MentorU.ViewModels
{
    public class GroupMainChatViewModel:BaseViewModel
    {
        public ObservableCollection<GroupMessages> Chats { get; }
        public Command LoadChatsCommand { get; }
        public Command<GroupMessages> ChatTapped { get; }

        public Command CreateGroupChat { get; }

        public Command CloseCreateChat { get; set; }
        public Command ClosePopUp { get; set; }


        private bool _noChats;
        public bool NoChats
        {
            get => _noChats;
            set
            {
                _noChats = value;
                OnPropertyChanged();
            }

        }

        private string _groupChatName;
        public string GroupChatName
        {
            get => _groupChatName;
            set
            {
                _groupChatName = value;
                OnPropertyChanged();
            }
        }

        public GroupMainChatViewModel()
        {
            Title = "Class Chats";
            //_user.FirstName = "Wallace";
            Chats = new ObservableCollection<GroupMessages>();
            LoadChatsCommand = new Command(async () => await ExecuteLoadChats());
            ChatTapped = new Command<GroupMessages>(OpenChat);
            NoChats = true;
            CreateGroupChat = new Command(CreateGroup);
            CloseCreateChat = new Command(async () => await StartChat());
            ClosePopUp = new Command(async () => await ClosePopUpWindow());
        }

        async Task ExecuteLoadChats()
        {
            IsBusy = true;
            try
            {
                Chats.Clear();

                var groups = await DatabaseService.Instance.client.GetTable<GroupMessages>().ToListAsync();

                foreach (var g in groups)
                {
                    Chats.Add(g);
                }
                
                if (Chats.Count > 0)
                {
                    NoChats = false;
                }
                  
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }


        async void OpenChat(GroupMessages Group)
        {
            await Shell.Current.Navigation.PushAsync(new GroupChatPage(Group.GroupName));
        }

        async Task StartChat()
        {
            IsBusy = true;
            if (!string.IsNullOrEmpty(GroupChatName))
            {
                GroupMessages newChat = new GroupMessages()
                {
                    GroupName = GroupChatName,
                    Owner = App.loggedUser.id
                };

                var groups = await DatabaseService.Instance.client.GetTable<GroupMessages>().ToListAsync();

                bool badName = false;
                foreach (var g in groups)
                {
                    if (g.GroupName.Equals(GroupChatName))
                    {
                        badName = true;
                        break;
                    }
                }

                if(!badName)
                {
                    await DatabaseService.Instance.client.GetTable<GroupMessages>().InsertAsync(newChat);
                    await PopupNavigation.Instance.PopAllAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Group Name Used", "This name has been used, please enter a different group name.", "OK");
                }

            }
            else { 
                await Shell.Current.DisplayAlert("No Group Name", "Please enter a group name", "OK"); 
            }
        }

        async Task ClosePopUpWindow()
        {
            IsBusy = true;
            await PopupNavigation.Instance.PopAllAsync();
        }


        public void OnAppearing()
        {
            IsBusy = true;
        }

        async void CreateGroup()
        {
            await PopupNavigation.Instance.PushAsync(new PopUpGroupChat(this));
        }

    }
}
