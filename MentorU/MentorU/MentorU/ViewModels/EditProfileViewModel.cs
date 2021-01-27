using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using MentorU.Models;
using Newtonsoft.Json.Linq;

namespace MentorU.ViewModels
{
    public class EditProfileViewModel : ProfileViewModel
    {
        private ProfileViewModel _parentVM;
        private string _newClass;
        private string _department;
        private Dictionary<string, List<string>> _catalog;
        private List<string> _allClasses;
        public Command SaveButtonCommand { get; set; }
        public Command CancelButtonCommand { get; set; }
        public Command AddClassCommand { get; set; }
        public Command RemoveClassCommand { get; set; }
        public Command AddProfilePictureCommand { get; set; }

        public List<string> AllDepartments { get; set; }

        public List<string> AllClasses
        {
            get => _allClasses; 
            set
            {
                _allClasses = value;
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
                if(_department != "None")
                    AllClasses = _catalog[_department];
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

            _catalog = new Dictionary<string, List<string>>()
            {
                { "CS", new List<string>() { "CS 1410", "CS 3500", "CS 4300" } },
                {"CHEM", new List<string>() {"CHEM 1210", "CHEM 2420" } },
                {"MATH", new List<string>() {"MATH 1210", "MATH 2420", "MATH 2700" } }
            };

            AllClasses = new List<string>();
            AllDepartments = new List<string>() { "None","CS", "MATH", "CHEM" };
            AddClassCommand = new Command(AddClass);
            RemoveClassCommand = new Command(async () => await RemoveClass());
            SaveButtonCommand = new Command(OnSave);
            CancelButtonCommand = new Command(OnCancel);
            AddProfilePictureCommand = new Command(AddPicture);
            Department = "None";
        }


        private void AddClass()
        {
            if(!string.IsNullOrEmpty(NewClass))
            {
                Classes.Add(NewClass);
                NewClass = "";
            }
        }

        private async Task RemoveClass()
        {
            if(OldClass != null)
            {
                bool confirmed = await Application.Current.MainPage.DisplayAlert("Confirmation",$"Do you want to remove {OldClass}","Yes","No");
                if(confirmed)
                    Classes.Remove(OldClass);
            }
        }

        private async void OnSave()
        {
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
            await App.client.GetTable<Users>().UpdateAsync(data);
            await Shell.Current.Navigation.PopModalAsync();
        }

        private async void OnCancel()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }

        private async void AddPicture()
        {
            
        }
    }
}
