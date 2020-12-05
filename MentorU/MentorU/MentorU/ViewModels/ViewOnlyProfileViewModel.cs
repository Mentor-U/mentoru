using MentorU.Models;
using MentorU.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    [QueryProperty(nameof(UserID), nameof(UserID))]
    public class ViewOnlyProfileViewModel : BaseViewModel
    {
        private string name;
        private string field;
        private string bio;
        private string userID;
        public Command StartChatCommand { get; }
        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Field { get => field; set => SetProperty(ref field, value); }
        public string Bio { get => bio; set => SetProperty(ref bio, value); }
        public string UserID
        {
            get => userID;
            set
            {
                userID = value;
                LoadUserID(value);
            }
        }

        public ViewOnlyProfileViewModel()
        {
            StartChatCommand = new Command(StartChat);
        }

        public async void LoadUserID(string id)
        {
            try
            {
                Users user = await DataStore.GetUser(int.Parse(id));
                Name = user.Name;
                Field = user.Major;
                Bio = user.Bio;
            }
            catch (Exception)
            {
                Debug.WriteLine("Unable to get mentor for viewing");
            }
        }

        private async void StartChat(object obj)
        {
            // TODO: pass userID to chat page to open the conversation
            await Shell.Current.Navigation.PopToRootAsync(false); // false -> disables navigation animation
            await Shell.Current.GoToAsync(nameof(MainChatPage));
        }

    }
}
