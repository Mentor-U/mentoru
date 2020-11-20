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

        /* Attributes from the user that are needed for dispaly */
        public string Name { get => _user.Name; }
        public string Major { get => _user.Major; }
        public ObservableCollection<string> Classes { get; }
        public string Bio { get => _user.Bio; }

        /***
         * Constructor. Initialize bindings from view
         */
        public ProfileViewModel()
        {
            _user = new User("Wallace");
            Title = "Profile";
            EditProfileCommmand = new Command(EditProfile);
        }

        public async void EditProfile()
        {
            // TODO: navigate to the edit view to make changes to the user. 
            // changes should be set up to automatically update.
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
