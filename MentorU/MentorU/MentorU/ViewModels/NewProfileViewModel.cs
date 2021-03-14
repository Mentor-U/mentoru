using System.Collections.Generic;
using System.Collections.ObjectModel;
using MentorU.Models;
using MentorU.Services.DatabaseServices;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using MentorU.Services;
using Azure.Storage.Blobs;
using MentorU.Services.Blob;
using System;
using System.Diagnostics;

namespace MentorU.ViewModels
{
    public class NewProfileViewModel : BaseViewModel
    {
        private string _name;
        private string _major;
        private string _bio;
        private string _newClass;
        private string _department;

        private Regex _depRegex = new Regex(@"([A-Za-z]+\s*)+");
        private Regex _courseRegex = new Regex(@"[0-9]+");

        public List<string> AllDepartments { get; set; }
        public ObservableCollection<string> Classes {get; set; }
        public string OldClass { get; set; }

        public Command AddClassCommand { get; set; }
        public Command RemoveClassCommand { get; set; }
        public Command AddProfilePictureCommand { get; set; }
        public Command DoneCommand { get; set; }

        public Command LoadPageCommand { get; set; }
        
        public Command isMentor { get; set; }
        public Command isMentee { get; set; }

        private bool _menteeView { get; set; }
        public bool MenteeView
        {
            get => _menteeView;
            set
            {
                _menteeView = value;
                OnPropertyChanged();
            }
        }

        private bool _mentorView { get; set; }
        public bool MentorView
        {
            get => _mentorView;
            set
            {
                _mentorView = value;
                OnPropertyChanged();
            }
        }

        private Color menteeColor { get; set; }
        private Color mentorColor { get; set; }
        public Color SelectedMentee
        {
            get => menteeColor;

            set
            {
                menteeColor = value;
                OnPropertyChanged();
            }
        }

        public Color SelectedMentor
        {
            get => mentorColor;
            set
            {
                mentorColor = value;
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

        private ImageSource _profileImage;
        private string profileImageFilePath;

        public ImageSource ProfileImage
        {
            get => _profileImage;
            set
            {
                _profileImage = value;
                OnPropertyChanged();
            }
        }

        public string NewClass
        {
            get => _newClass;
            set
            {
                _newClass = value;
                OnPropertyChanged();
            }

        }

        public string Department
        {
            get => _department;
            set
            {
                _department = value;
                OnPropertyChanged();
            }
        }


        public NewProfileViewModel()
        {
            Name = App.loggedUser.DisplayName;

            AllDepartments = new List<string>(DatabaseService.Instance.ClassList.classList);
            Department = AllDepartments[0];
            Classes = new ObservableCollection<string>();

            isMentee = new Command(RunAsMentee);
            isMentor = new Command(RunAsMentor);
            DoneCommand = new Command(Completed);
            AddClassCommand = new Command(AddClass);
            RemoveClassCommand = new Command(RemoveClass);
            LoadPageCommand = new Command(async () => await LoadPage());
            AddProfilePictureCommand = new Command(AddPicture);

            RunAsMentee();
        }

        async Task LoadPage()
        {
            IsBusy = false;
        }

        void RunAsMentee()
        {
            MenteeView = true;
            MentorView = false;
            SelectedMentee = Color.Red;
            SelectedMentor = Color.Gray;
        }

        void RunAsMentor()
        {
            MenteeView = false;
            MentorView = true;
            SelectedMentor = Color.Red;
            SelectedMentee = Color.Gray;
        }

        void AddClass()
        {
            Classes.Add(Department + " " + NewClass);
            NewClass = "";
        }

        void RemoveClass()
        {
            Classes.Remove(OldClass);
        }


        private async void AddPicture()
        {
            try
            {
                Stream profileImageStream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
                if (profileImageStream != null)
                {
                    string fileName = $"{App.loggedUser.id}--ProfileImage";
                    profileImageFilePath = DependencyService.Get<IFileService>().SavePicture(fileName, profileImageStream);

                    ProfileImage = profileImageFilePath;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

        }

        /// <summary>
        /// Save all the changes and insert into the DB
        /// </summary>
        async void Completed()
        {
            App.loggedUser.Major = Major;
            App.loggedUser.Bio = Bio;
            App.loggedUser.Role = MenteeView ? "1" : "0";

            try 
            {
                await DatabaseService.Instance.client.GetTable<Users>().InsertAsync(App.loggedUser);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            foreach (var c in Classes)
            {
                string dep = _depRegex.Match(c).Value;
                string cou = _courseRegex.Match(c).Value;
                if (MenteeView)
                    await DatabaseService.Instance.client.GetTable<Classes>()
                       .InsertAsync(new Models.Classes() { UserId = App.loggedUser.id, Department = dep, Course = cou, ClassName = c });
                else
                {
                    await DatabaseService.Instance.client.GetTable<Classes>()
                        .InsertAsync(new Models.Classes() { UserId = App.loggedUser.id, ClassName = c });
                }
            }

            try
            {
                BlobContainerClient containerClient = BlobService.Instance.BlobServiceClient.GetBlobContainerClient("profile-images");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
           

            await Shell.Current.GoToAsync("///Home");
            //await Application.Current.MainPage.Navigation.PopModalAsync();
        }


        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
