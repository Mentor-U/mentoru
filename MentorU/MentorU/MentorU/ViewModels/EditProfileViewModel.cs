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

namespace MentorU.ViewModels
{
    public class EditProfileViewModel : ProfileViewModel
    {
        private ProfileViewModel _parentVM;
        private string _newClass;

        private Regex _depRegex = new Regex(@"([A-Za-z]+\s*)+");
        private Regex _courseRegex = new Regex(@"(\d+)");

        private List<string> _addedClass;
        private List<string> _removedClass;

        private string _department;
        public List<string> AllDepartments { get; set; }

        public Command SaveButtonCommand { get; set; }
        public Command CancelButtonCommand { get; set; }
        public Command AddClassCommand { get; set; }
        public Command RemoveClassCommand { get; set; }
        public Command AddProfilePictureCommand { get; set; }

        private Image profileImage;
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


        /***
         * Allows for changes to the users profile and inherits the state of
         * the ProfileViewModel to allow the changes to be reflected in the ProfileView
         * if the are saved.
         */
        public EditProfileViewModel(ProfileViewModel profileVM)
        {
            _parentVM = profileVM;
            Name = App.loggedUser.FirstName;
            Major = App.loggedUser.Major;
            Bio = App.loggedUser.Bio;
            Classes = _parentVM.Classes;

            AllDepartments = new List<string>(DatabaseService.ClassList.classList);
            Department = AllDepartments[0];

            _addedClass = new List<string>();
            _removedClass = new List<string>();

            AddClassCommand = new Command(AddClass);
            RemoveClassCommand = new Command(async () => await RemoveClass());
            SaveButtonCommand = new Command(OnSave);
            CancelButtonCommand = new Command(OnCancel);
            AddProfilePictureCommand = new Command(AddPicture);
        }


        private void AddClass()
        {
            if(!string.IsNullOrEmpty(NewClass))
            {
                if (Department == AllDepartments[0] && isMentee)
                    Application.Current.MainPage.DisplayAlert("Attention", "Please select a department", "Ok");
                else
                {
                    if(isMentee)
                        NewClass = Department + " " + NewClass;
                    _removedClass.Remove(NewClass);// ensure NewClass does not exist in the remove list
                    Classes.Add(NewClass);
                    _addedClass.Add(NewClass);
                    NewClass = "";
                }
            }
        }

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

        private async void OnSave()
        {
            _parentVM.ProfileImage = ProfileImage;
            
            string fileName = App.loggedUser.id;

            // check if blob exists, if so delete
            await _parentVM.containerClient.DeleteBlobIfExistsAsync(fileName);
            await _parentVM.containerClient.UploadBlobAsync(fileName, File.OpenRead(profileImageFilePath));

            App.loggedUser.FirstName = _parentVM.Name = Name;
            App.loggedUser.Major = _parentVM.Major = Major;
            App.loggedUser.Bio = _parentVM.Bio = Bio;
            _parentVM.Classes = Classes;
            JObject data = new JObject
            {
                {"id", App.loggedUser.id },
                {"FirstName", Name },
                {"Major", Major },
                {"Bio", Bio }
            };
            await DatabaseService.client.GetTable<Users>().UpdateAsync(data);

            foreach (var c in _removedClass)
            {
                var cl = await DatabaseService.client.GetTable<Classes>()
                    .Where(u => u.UserId == App.loggedUser.id && u.ClassName == c).ToListAsync();
                if(cl.Count > 0)
                    await DatabaseService.client.GetTable<Classes>().DeleteAsync(cl[0]);

            }

            foreach (var c in _addedClass)
            {
                string dep = _depRegex.Match(c).Value;
                string cou = _courseRegex.Match(c).Value;
                if(isMentee)
                    await DatabaseService.client.GetTable<Classes>()
                       .InsertAsync(new Models.Classes() { UserId = App.loggedUser.id, Department = dep, Course = cou, ClassName = c });
                else
                {
                    await DatabaseService.client.GetTable<Classes>()
                        .InsertAsync(new Models.Classes() { UserId = App.loggedUser.id, ClassName = c });
                }
            }

            await Shell.Current.Navigation.PopModalAsync();
        }

        private async void OnCancel()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }

        private async void AddPicture()
        {

            Stream profileImageStream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (profileImageStream != null)
            {
                string fileName = $"{App.loggedUser.id}--ProfileImage";
                profileImageFilePath = DependencyService.Get<IFileService>().SavePicture(fileName, profileImageStream);

                ProfileImage = profileImageFilePath;
            }

        }
    }
}
