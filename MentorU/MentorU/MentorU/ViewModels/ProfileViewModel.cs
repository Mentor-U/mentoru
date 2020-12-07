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
        private Users _user;
        private string _name;
        private string _major;
        private string _bio;
         
        public Command EditProfileCommand { get; }
        public Command LoadPageDataCommand { get; }
        public Command<Users> MentorTapped { get; }

        /* Attributes from the user that are needed for dispaly */
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Major
        { 
            get => _major;
            set
            {
                _major = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> Classes { get; }
        public ObservableCollection<Users> Mentors { get; }
        public string Bio
        {
            get => _bio;
            set
            {
                _bio = value;
                OnPropertyChanged();
            }
        }

        /***
         * Constructor. Initialize bindings from view
         */
        public ProfileViewModel()
        {

            Name = App.loggedUser.FirstName + App.loggedUser.LastName;
            Major = App.loggedUser.Major;
            Bio = App.loggedUser.Bio;
            Title = "Profile";
            Mentors = new ObservableCollection<Users>();

            LoadPageDataCommand = new Command(async () => await ExecuteLoad()); // fetch all data 
            EditProfileCommand = new Command(EditProfile);
            MentorTapped = new Command<Users>(OnMentorSelected);
        }

        async Task ExecuteLoad() 
        {
            IsBusy = true;
            try
            {
                Mentors.Clear(); // mentor list
                //if mentor
                if(App.loggedUser.Role == 0) { return; }
                
                var mentors = await DataStore.GetMentorsAsync(true); 
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
            await Shell.Current.Navigation.PushModalAsync(new EditProfilePage(this));
        }

        async void OnMentorSelected(Users mentor)
        {
            if (mentor == null)
                return;
            await Shell.Current.GoToAsync($"{nameof(ViewOnlyProfilePage)}?{nameof(ViewOnlyProfileViewModel.UserID)}={mentor.id}");
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
