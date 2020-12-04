using MentorU.Models;
using MentorU.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class MainChatViewModel : BaseViewModel
    {
        private Profile _user;
        public ObservableCollection<Profile> Chats { get; }
        public Command LoadChatsCommand { get; }
        public MainChatViewModel()
        {
            Title = "Chats";
            _user.Name = "Wallace";
            Chats = new ObservableCollection<Profile>();
            LoadChatsCommand = new Command(async () => await ExecuteLoadChats());
        }

        async Task ExecuteLoadChats()
        {
            IsBusy = true;
            try
            {
                Chats.Clear();
                // var chats = await Data.GetChatsAsync(true);
                //User u1 = new User("George");
                //User u2 = new User("Steve");
                Profile u1 = new Profile { Name = "George" };
                Profile u2 = new Profile { Name = "Steve" };
                Chats.Add(u1);
                Chats.Add(u2);
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

        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
