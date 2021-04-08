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

namespace MentorU.ViewModels
{
    public class MainChatViewModel : BaseViewModel
    {
        public ObservableCollection<Users> Chats { get; }
        public Command LoadChatsCommand { get; }
        public Command<Users> UserTapped { get; }
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

        public MainChatViewModel()
        {
            Title = "Chats";
            //_user.FirstName = "Wallace";
            Chats = new ObservableCollection<Users>();
            LoadChatsCommand = new Command(async () => await ExecuteLoadChats());
            UserTapped = new Command<Users>(OpenChat);
            NoChats = true;
        }

        async Task ExecuteLoadChats()
        {
            IsBusy = true;
            try
            {
                Chats.Clear();

                List<Users> mentor_list = new List<Users>();

                //Adds only mentors that you have connected with
                if (App.loggedUser.Role == "0")
                {
                    List<Connection> cons = await DatabaseService.Instance.client.GetTable<Connection>()
                        .Where(u => u.MentorID == App.loggedUser.id).ToListAsync();
                    foreach (Connection c in cons)
                    {
                        List<Users> temp = await DatabaseService.Instance.client.GetTable<Users>()
                            .Where(u => u.id == c.MenteeID).ToListAsync();
                        var current = temp[0];
                        current.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", current.id);
                        Chats.Add(current);
                    }
                }
                else
                {
                    List<Connection> cons = await DatabaseService.Instance.client.GetTable<Connection>()
                        .Where(u => u.MenteeID == App.loggedUser.id).ToListAsync();
                    foreach (Connection c in cons)
                    {
                        List<Users> temp = await DatabaseService.Instance.client.GetTable<Users>()
                            .Where(u => u.id == c.MentorID).ToListAsync();
                        var current = temp[0];
                        current.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", current.id);
                        Chats.Add(current);
                    }
                }

                if (Chats.Count > 0)
                    NoChats = false;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }


        async void OpenChat(Users ChatRecipient)
        {
            await Shell.Current.Navigation.PushAsync(new ChatPage(ChatRecipient));
        }


        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
