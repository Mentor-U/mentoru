using MentorU.Models;
using MentorU.Views.ChatViews;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class MainChatViewModel : BaseViewModel
    {
        public ObservableCollection<Users> Chats { get; }
        public Command LoadChatsCommand { get; }
        public Command<Users> UserTapped { get; }


        public MainChatViewModel()
        {
            Title = "Chats";
            //_user.FirstName = "Wallace";
            Chats = new ObservableCollection<Users>();
            LoadChatsCommand = new Command(async () => await ExecuteLoadChats());
            UserTapped = new Command<Users>(OpenChat);
        }

        async Task ExecuteLoadChats()
        {
            IsBusy = true;
            try
            {
                Chats.Clear();
                List<Users> mentor_list = new List<Users>();
                if (App.loggedUser.Role == "1")
                    mentor_list = await App.client.GetTable<Users>().Where(user => user.Role == "0").ToListAsync();
                else
                    mentor_list = await App.client.GetTable<Users>().Where(user => user.Role == "1").ToListAsync();
                foreach (Users m in mentor_list)
                {
                    Chats.Add(m);
                }
                //if (App.loggedUser.Role == "0")
                //{
                //    List<Connection> cons = await App.client.GetTable<Connection>().Where(u => u.MentorID == App.loggedUser.id).ToListAsync();
                //    foreach (Connection c in cons)
                //    {
                //        List<Users> temp = await App.client.GetTable<Users>().Where(u => u.id == c.MenteeID).ToListAsync();
                //        Chats.Add(temp[0]);
                //    }
                //}
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
