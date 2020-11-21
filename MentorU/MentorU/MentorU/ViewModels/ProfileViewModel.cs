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
        public string Name { get; set; }
        public string Major { get; set; }
        public ObservableCollection<string> Classes { get; }
        public string Bio { get; set; }

        /***
         * Constructor. Initialize bindings from view
         */
        public ProfileViewModel(User user)
        {
            _user = user;
            Name = _user.Name;
            Major = _user.Major;
            Bio = _user.Bio;
            Title = "Profile";
            EditProfileCommmand = new Command(EditProfile);
        }

        public async void EditProfile()
        {
            EditProfileViewModel editProfileVM = new EditProfileViewModel(ref _user);
            EditProfilePage editProfilePage = new EditProfilePage
            {
                BindingContext = editProfileVM
            };
            await Application.Current.MainPage.Navigation.PushModalAsync(editProfilePage);
        }

        public void OnAppearing()
        {
            IsBusy = false;
        }
    }
}
