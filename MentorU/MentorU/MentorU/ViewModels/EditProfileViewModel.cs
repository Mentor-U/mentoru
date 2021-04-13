using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using MentorU.Models;
using Newtonsoft.Json.Linq;
using MentorU.Services.DatabaseServices;
using System.Text.RegularExpressions;
using System.IO;
using MentorU.Services;
using Azure.Storage.Blobs;
using MentorU.Services.Blob;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace MentorU.ViewModels
{
    public class EditProfileViewModel : ProfileViewModel
    {
        private ProfileViewModel _parentVM;
        private string _newClass;

        private Regex _depRegex = new Regex(@"([A-Za-z]+\s*)+");
        private Regex _courseRegex = new Regex(@"[0-9]+");

        private List<string> _addedClass;
        private List<string> _removedClass;

        private string _department;
        public List<string> AllDepartments { get; set; }

        public Command SaveButtonCommand { get; set; }
        public Command CancelButtonCommand { get; set; }
        public Command AddClassCommand { get; set; }
        public Command RemoveClassCommand { get; set; }
        public Command AddProfilePictureCommand { get; set; }

        private bool _imageChanged { get; set; }

        private string profileImageFilePath;

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


        public string OldClass { get; set; }


        /// <summary>
        /// Allows for changes to the users profile and inherits the state of
        /// the ProfileViewModel to allow the changes to be reflected in the ProfileView
        /// if the are saved.
        /// </summary>
        /// <param name="profileVM"></param>
        public EditProfileViewModel(ProfileViewModel profileVM)
        {
            _parentVM = profileVM;
            Name = App.loggedUser.FirstName;
            Major = App.loggedUser.Major;
            Bio = App.loggedUser.Bio;
            Classes = _parentVM.Classes;
            _imageChanged = false;
            //ProfileImage = new Task<ImageSource>(async () => {
            //    await BlobService.Instance.TryDownloadImage("profile-images", App.loggedUser.id);
            //    });

            
            AllDepartments = new List<string>(DatabaseService.Instance.ClassList.classList);
            Department = AllDepartments[0];

            _addedClass = new List<string>();
            _removedClass = new List<string>();

            AddClassCommand = new Command(AddClass);
            RemoveClassCommand = new Command(async () => await RemoveClass());
            SaveButtonCommand = new Command(OnSave);
            CancelButtonCommand = new Command(OnCancel);
            AddProfilePictureCommand = new Command(AddPicture);
        }

        /// <summary>
        /// Add a class to a temporary list that will replace the current list
        /// if the user selects save
        /// </summary>
        private void AddClass()
        {
            if(!string.IsNullOrEmpty(NewClass))
            {
                if (Department == AllDepartments[0] && isMentee)
                    Application.Current.MainPage.DisplayAlert("Attention", "Please select a department", "Ok");
                else
                {
                    string addClass = _newClass;
                    if(isMentee)
                        addClass = (Department + " " + addClass);
                    _removedClass.Remove(addClass);// ensure NewClass does not exist in the remove list
                    Classes.Add(addClass);
                    _addedClass.Add(addClass);
                    NewClass = "";
                }
            }
        }

        /// <summary>
        /// Remove the 'OldClass', which is selected by the user, from the
        /// temporary class list
        /// </summary>
        /// <returns></returns>
        private async Task RemoveClass()
        {
            if(OldClass != null)
            {
                bool confirmed = await Application.Current.MainPage.DisplayAlert("Confirmation",$"Do you want to remove {OldClass}","Yes","No");
                if (confirmed)
                {
                    _addedClass.Remove(OldClass); // ensure OldClass does not exist in the add list
                    Classes.Remove(OldClass);
                    _removedClass.Add(OldClass);
                }
            }
        }

        /// <summary>
        /// Update the UI and save the changes to the database.
        /// </summary>
        private async void OnSave()
        {

            if (_imageChanged)
            {
                string tempFileName = $"{App.loggedUser.id}--ProfileImage-tmp";
                string fileName = App.loggedUser.id;

                BlobContainerClient containerClient = BlobService.Instance.BlobServiceClient.GetBlobContainerClient("profile-images");
                BlobClient blobTempProf = containerClient.GetBlobClient(tempFileName);

                // Download new/temp profile image to a stream, re upload it as the users profile image (deletes the tmp tag)
                using( var ms = new MemoryStream())
                {
                    if (blobTempProf.Exists())
                    {
                        await blobTempProf.DownloadToAsync(ms);
                    }

                    // Reset strem pos since its at the end after downloading
                    ms.Position = 0;
                    // Re upload the temp image as the profile image now
                    if(await BlobService.Instance.TryUploadImageStream(containerClient, fileName, ms))
                    {
                        _parentVM.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", fileName);
                    }

                }

                // Set the profile image away from the temp blob and delete the temp
                await containerClient.DeleteBlobIfExistsAsync(tempFileName);
                _imageChanged = false;
            }

            App.loggedUser.DisplayName = _parentVM.Name = Name;
            App.loggedUser.Major = _parentVM.Major = Major;
            App.loggedUser.Bio = _parentVM.Bio = Bio;
            _parentVM.Classes = Classes;
            JObject data = new JObject
            {
                {"id", App.loggedUser.id },
                {"DisplayName", Name },
                {"Major", Major },
                {"Bio", Bio }
            };
            await DatabaseService.Instance.client.GetTable<Users>().UpdateAsync(data);

            foreach (var c in _removedClass)
            {
                var cl = await DatabaseService.Instance.client.GetTable<Classes>()
                    .Where(u => u.UserId == App.loggedUser.id && u.ClassName == c).ToListAsync();
                if(cl.Count > 0)
                    await DatabaseService.Instance.client.GetTable<Classes>().DeleteAsync(cl[0]);

            }

            foreach (var c in _addedClass)
            {
                string dep = _depRegex.Match(c).Value;
                string cou = _courseRegex.Match(c).Value;
                if(isMentee)
                    await DatabaseService.Instance.client.GetTable<Classes>()
                       .InsertAsync(new Models.Classes() { UserId = App.loggedUser.id, Department = dep, Course = cou, ClassName = c });
                else
                {
                    await DatabaseService.Instance.client.GetTable<Classes>()
                        .InsertAsync(new Models.Classes() { UserId = App.loggedUser.id, ClassName = c });
                }
            }

            await Shell.Current.Navigation.PopModalAsync();
        }


        /// <summary>
        /// Discard the changes and return to the profile page
        /// </summary>
        private async void OnCancel()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }


        /// <summary>
        /// Open the native photos application and allow images to be add to the application.
        /// </summary>
        private async void AddPicture()
        {

            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await AppShell.Current.DisplayAlert("Not supported", "Your device does not currently support this functionality", "Ok");
                return;
            }

            var mediaOption = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };

            string fileName = $"{App.loggedUser.id}--ProfileImage-tmp";

            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOption);

            if (selectedImageFile == null) return;

            BlobContainerClient containerClient = BlobService.Instance.BlobServiceClient.GetBlobContainerClient("profile-images");

            await BlobService.Instance.TryUploadImageStream(containerClient, fileName, selectedImageFile.GetStream());

            //profileImageFilePath = DependencyService.Get<IFileService>().SavePicture(fileName, selectedImageFile.GetStream());

            ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", fileName);
            _imageChanged = true;

        }



        public new async Task OnAppearing()
        {
            IsBusy = true;
            //containerClient = BlobService.Instance.BlobServiceClient.GetBlobContainerClient();
            //await GetProfileImage();
            ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", App.loggedUser.id);
        }
    }
}
