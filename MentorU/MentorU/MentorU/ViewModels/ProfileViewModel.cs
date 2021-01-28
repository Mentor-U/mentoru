using MentorU.Models;
using MentorU.Services.DatabaseServices;
using MentorU.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _name;
        private string _major;
        private string _bio;
        private string _classes;

        public bool isMentor { get; set; }
        public bool isMentee { get; set; }

        public ObservableCollection<string> Classes { get; set; }
        public ObservableCollection<Users> Mentors { get; set; }

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

            if(App.loggedUser.Role == "0")
            {
                isMentor = true;
                isMentee = false;
            }
            else
            {
                isMentor = false;
                isMentee = true;
            }

            Name = App.loggedUser.FirstName + " " + App.loggedUser.LastName;
            Major = App.loggedUser.Major;
            Bio = App.loggedUser.Bio;

            Title = "Profile";

            Mentors = new ObservableCollection<Users>();
            Classes = new ObservableCollection<string>();

            LoadPageDataCommand = new Command(async () => await ExecuteLoad()); // fetch all data 
            EditProfileCommand = new Command(EditProfile);
            MentorTapped = new Command<Users>(OnMentorSelected);

            //REMOVE: once database contains the classes information
            Classes.Add("CS 1410");
            Classes.Add("CS 3500");
            //Classes.Add("CS 2420");
        }

        protected async Task ExecuteLoad() 
        {
            IsBusy = true;
            try
            {
                Mentors.Clear(); // mentor list
                //if mentor
                //if(App.loggedUser.Role == 0) { return; }

                List<Connection> mentors;

                if(isMentee)
                {
                    mentors = await DatabaseService.client.GetTable<Connection>().Where(u => u.MenteeID == App.loggedUser.id).ToListAsync();
                    foreach (var m in mentors)
                    {
                        var men = await DatabaseService.client.GetTable<Users>().Where(u => u.id == m.MentorID).ToListAsync();
                        Mentors.Add(men[0]);
                    }
                }
                else
                {
                    mentors = await DatabaseService.client.GetTable<Connection>().Where(u => u.MentorID == App.loggedUser.id).ToListAsync();
                    foreach (var m in mentors)
                    {
                        var men = await DatabaseService.client.GetTable<Users>().Where(u => u.id == m.MenteeID).ToListAsync();
                        Mentors.Add(men[0]);
                    }
                }

                
               // var mentors = await DataStore.GetMentorsAsync(true); 
                

                // TODO: Add class loading here from data base or logged user. what ever we decide on
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
            await Shell.Current.Navigation.PushAsync(new ViewOnlyProfilePage(mentor, true));
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
