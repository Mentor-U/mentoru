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
        private Users _user;
        public ObservableCollection<Users> Chats { get; }
        public Command LoadChatsCommand { get; }
        public MainChatViewModel()
        {
            Title = "Chats";
            _user.Name = "Wallace";
            Chats = new ObservableCollection<Users>();
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
                Users u1 = new Users { Name = "George" };
                Users u2 = new Users { Name = "Steve" };
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
