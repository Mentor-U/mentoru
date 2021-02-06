using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MentorU.Models;
using MentorU.Services.DatabaseServices;
using MentorU.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _name;
        private string _major;
        private string _bio;
        private ImageSource _profileImage;

        public bool isMentor { get; set; }
        public bool isMentee { get; set; }

        public ObservableCollection<string> Classes { get; set; }
        public ObservableCollection<Users> Mentors { get; set; }


        public Command EditProfileCommand { get; }
        public Command LoadPageDataCommand { get; }
        public Command<Users> MentorTapped { get; }


        private string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=mentorustorage;AccountKey=gbaR9b3iwGtWfSjbWKW5mgC1mtEpU2UijjOrwrniFaAS8Kb0KLr/g4inZX6+aoNB07FoUUSR4hxYYP7ZTNbbfw==;EndpointSuffix=core.windows.net";
        
        public BlobServiceClient client;
        public BlobContainerClient containerClient;
        public BlobClient blobClient;

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
            
        }

        private async Task GetProfileImage()
        {
            
            BlobClient blob = containerClient.GetBlobClient(App.loggedUser.id);
            
            if(blob.Exists())
            {
                BlobDownloadInfo info = await blob.DownloadAsync();

                ProfileImage = ImageSource.FromStream(() => info.Content);
            }
            else
            {
                ProfileImage = "placeholder.jpg";
            }
        }


        /// <summary>
        /// Loads all the information to be displayed from the database
        /// Executed everytime the page is view or refreshed.
        /// </summary>
        /// <returns></returns>
        protected async Task ExecuteLoad() 
        {
            IsBusy = true;
            try
            {
                Mentors.Clear(); // mentor list
                Classes.Clear();

                //if mentor
                //if(App.loggedUser.Role == 0) { return; }

                // Load all connections
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

                // Redirect option to browse for new mentors -1 role so clicked event can react appropriately
                if (Mentors.Count == 0)
                {
                    Mentors.Add(new Users() { FirstName = "No current connections", Major = "Click to browse  list", Role = "-1" });
                }

                //Load all classes
                List<Classes> c = await DatabaseService.client.GetTable<Classes>().Where(u => u.UserId == App.loggedUser.id).ToListAsync();
                foreach(Classes val in c)
                {
                    Classes.Add(val.ClassName);
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


        public async Task OnAppearing()
        {
            IsBusy = true;
            string containerName = "profile-images";

            client = new BlobServiceClient(storageConnectionString);
            containerClient = client.GetBlobContainerClient(containerName);
            await GetProfileImage();
        }
    }
}
