using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MentorU.Models;
using MentorU.Services.Blob;
using MentorU.Services.DatabaseServices;
using MentorU.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Extensions.Caching.Memory;

namespace MentorU.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _name;
        private string _major;
        private string _bio;
        private string _email;
        private bool _showEmail;

        private ImageSource _profileImage;

        public bool isMentor { get; set; }
        public bool isMentee { get; set; }

 

        public ObservableCollection<string> Classes { get; set; }
        public ObservableCollection<Users> Mentors { get; set; }
        public ObservableCollection<Items> Marketplace { get; set; }


        public Command EditProfileCommand { get; }
        public Command LoadPageDataCommand { get; }
        public Command<Users> MentorTapped { get; }
        public Command<Items> ItemTapped { get; }

        /* Attributes from the user that are needed for dispaly */
        public ImageSource ProfileImage
        {
            get => _profileImage;
            set
            {
                _profileImage = value;
                OnPropertyChanged();
            }
        }

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

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public bool showEmail
        { 
          get => _showEmail; 
          set 
            {
                _showEmail = value;  OnPropertyChanged(); 
            }
        }


        /***
         * Constructor. Initialize bindings from view
         */
        public ProfileViewModel()
        {
            IsBusy = true;
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

            Name = App.loggedUser.FirstName;
            Major = App.loggedUser.Major;
            Bio = App.loggedUser.Bio;



            Title = "Profile";

            

            Mentors = new ObservableCollection<Users>();
            Classes = new ObservableCollection<string>();
            Marketplace = new ObservableCollection<Items>();

            LoadPageDataCommand = new Command(async () => await ExecuteLoad()); // fetch all data 
            EditProfileCommand = new Command(EditProfile);
            MentorTapped = new Command<Users>(OnMentorSelected);
            ItemTapped = new Command<Items>(OnItemSelected);
            
        }

      


        /// <summary>
        /// Loads all the information to be displayed from the database
        /// Executed everytime the page is view or refreshed.
        /// </summary>
        /// <returns></returns>
        protected async Task ExecuteLoad() 
        {
            try
            {
                ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", App.loggedUser.id);
                Mentors.Clear(); // mentor list
                Classes.Clear();
                Marketplace.Clear();

                //if mentor
                //if(App.loggedUser.Role == 0) { return; }

                // Load all connections
                List<Connection> mentors;
                if(isMentee)
                {
                    mentors = await DatabaseService.Instance.client.GetTable<Connection>().Where(u => u.MenteeID == App.loggedUser.id).ToListAsync();
                    foreach (var m in mentors)
                    {
                        var men = await DatabaseService.Instance.client.GetTable<Users>().Where(u => u.id == m.MentorID).ToListAsync();
                        var current = men[0];
                        current.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", current.id);
                        Mentors.Add(current);
                    }
                }
                else
                {
                    mentors = await DatabaseService.Instance.client.GetTable<Connection>().Where(u => u.MentorID == App.loggedUser.id).ToListAsync();
                    foreach (var m in mentors)
                    {
                        var men = await DatabaseService.Instance.client.GetTable<Users>().Where(u => u.id == m.MenteeID).ToListAsync();
                        var current = men[0];
                        current.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", current.id);
                        Mentors.Add(current);
                    }
                }

                // Redirect option to browse for new mentors -1 role so clicked event can react appropriately
                if (Mentors.Count == 0)
                {
                    Mentors.Add(new Users() { FirstName = "No current connections", Major = "Click to browse  list", Role = "-1" });
                }

                //Load all classes
                List<Classes> c = await DatabaseService.Instance.client.GetTable<Classes>().Where(u => u.UserId == App.loggedUser.id).ToListAsync();
                foreach(Classes val in c)
                {
                    Classes.Add(val.ClassName);
                }

                //Load all marketplace items this user has listed
                List<Items> i = await DatabaseService.Instance.client.GetTable<Items>().Where(u => u.Owner == App.loggedUser.id).ToListAsync();
                foreach(Items val in i)
                {
                    Marketplace.Add(val);
                }

                var settingsList = await DatabaseService.Instance.client.GetTable<Settings>().Where(u => u.UserID == App.loggedUser.id).ToListAsync();
                Email = settingsList.Count > 0 ? App.loggedUser.Email : "";

                if(settingsList.Count > 0)
                {
                    if (settingsList[0].AllEmailSettings == true || settingsList[0].ConnectionEmailSettings == true)
                    {
                        showEmail = true;
                    }
  
                }
                else
                {
                    showEmail = false;
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
            else if (mentor.Role == "-1")
                await Shell.Current.Navigation.PushAsync(new SearchNewMentorPage());
            else
                await Shell.Current.Navigation.PushAsync(new ViewOnlyProfilePage(mentor, true));
        }

        async void OnItemSelected(Items item)
        {
            if (item == null)
                return;
            else
                await Shell.Current.Navigation.PushAsync(new ItemDetailPage(item));
        }


        public async Task OnAppearing()
        {
            //containerClient = BlobService.Instance.BlobServiceClient.GetBlobContainerClient();
            //await GetProfileImage();
            ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", App.loggedUser.id);
        }

    }
}
