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
            _user.FirstName = "Wallace";
            Chats = new ObservableCollection<Users>();
            LoadChatsCommand = new Command(async () => await ExecuteLoadChats());
        }

        async Task ExecuteLoadChats()
        {
            IsBusy = true;
            try
            {
                Chats.Clear();
                Users u1 = new Users { FirstName = "George" };
                Users u2 = new Users { FirstName = "Steve" };
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
