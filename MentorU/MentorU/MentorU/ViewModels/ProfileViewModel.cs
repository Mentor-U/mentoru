using MentorU.Models;
using MentorU.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private User _user;
        public Command EditProfileCommmand { get; set; }
        public Command LoadInfoCommand { get; }

        public string Name
        {
            get => _user.Name;
        }
        public string Major
        {
            get => _user.Major;
        }

        public ObservableCollection<string> Classes
        {
            get;
        }
        public string Bio
        {
            get => _user.Bio;
        }

        public ProfileViewModel()
        {
            _user = new User("jon");
            Title = "Profile";
            LoadInfoCommand = new Command(LoadInfo);
            EditProfileCommmand = new Command(EditProfile);
        }

        public async void EditProfile()
        {

        }

        public async void LoadInfo()
        {
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
