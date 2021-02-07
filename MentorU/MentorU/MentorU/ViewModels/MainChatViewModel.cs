using MentorU.Models;
using MentorU.Views.ChatViews;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using MentorU.Services.DatabaseServices;

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

                //Adds only mentors that you have connected with
                if (App.loggedUser.Role == "0")
                {
                    List<Connection> cons = await DatabaseService.Instance.client.GetTable<Connection>()
                        .Where(u => u.MentorID == App.loggedUser.id).ToListAsync();
                    foreach (Connection c in cons)
                    {
                        List<Users> temp = await DatabaseService.Instance.client.GetTable<Users>()
                            .Where(u => u.id == c.MenteeID).ToListAsync();
                        Chats.Add(temp[0]);
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
                        Chats.Add(temp[0]);
                    }
                }
                
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
