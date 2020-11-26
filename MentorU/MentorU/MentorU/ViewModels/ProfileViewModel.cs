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
        public Command<User> MentorTapped { get; }

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
            _user = DataStore.GetUser().Result;
            Title = "Profile";
            Mentors = new ObservableCollection<User>();
            
            //TODO: add all commands for loading market place recommendations and fetching user data from DB
            LoadPageDataCommand = new Command(async () => await ExecuteLoadMentors()); 
            EditProfileCommand = new Command(EditProfile);
            MentorTapped = new Command<User>(OnMentorSelected);
        }

        async Task ExecuteLoadMentors() //TODO: make async with the call to the datastore
        {
            IsBusy = true;
            try
            {
                Mentors.Clear();
                var mentors = await DataStore.GetMentorsAsync(); 
                foreach(var m in mentors)
                {
                    Mentors.Add(m);
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

        private async void EditProfile(object obj)
        {
            await Shell.Current.GoToAsync(nameof(EditProfilePage));
        }

        async void OnMentorSelected(User mentor)
        {
            // TODO: pass in the mentor that is wanting to be used as they are hard coded right now
            await Shell.Current.GoToAsync(nameof(ViewOnlyProfilePage));
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
