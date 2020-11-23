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
        public Command EditProfileCommand { get; }
        public Command LoadPageDataCommand { get; }

        /* Attributes from the user that are needed for dispaly */
        public string Name { get => _user.Name; }
        public string Major { get => _user.Major; }
        public ObservableCollection<string> Classes { get; }
        public ObservableCollection<User> Mentors { get; }
        public string Bio { get => _user.Bio; }

        /***
         * Constructor. Initialize bindings from view
         */
        public ProfileViewModel()
        {
            // TODO: get user from data base as they should already exist if they are on this page
            _user = new User("Wallace");
            Title = "Profile";
            Mentors = new ObservableCollection<User>();

            //TODO: add all commands for loading market place recommendations and fetching user data from DB
            LoadPageDataCommand = new Command(async () => await ExecuteLoadMentors()); 
            EditProfileCommand = new Command(EditProfile);
        }

        private async void EditProfile(object obj)
        {
            await Shell.Current.GoToAsync(nameof(EditProfilePage));
        }

        async Task ExecuteLoadMentors() //TODO: make async with the call to the datastore
        {
            IsBusy = true;
            try
            {
                Mentors.Clear();
                // var mentors = await DataStore.GetMentorsAsync(); // TODO: add datastore method
                User m1 = new User("George");
                User m2 = new User("Steve");
                Mentors.Add(m1);
                Mentors.Add(m2);
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
            IsBusy = false;
        }
    }
}
