using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using MentorU.Models;
using Newtonsoft.Json.Linq;
using MentorU.Services.DatabaseServices;


namespace MentorU.ViewModels
{
    public class EditProfileViewModel : ProfileViewModel
    {
        private ProfileViewModel _parentVM;
        private string _newClass;
        private List<string> _allClasses;

        public Command SaveButtonCommand { get; set; }
        public Command CancelButtonCommand { get; set; }
        public Command AddClassCommand { get; set; }
        public Command RemoveClassCommand { get; set; }
        public Command AddProfilePictureCommand { get; set; }


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

           

            AllClasses = new List<string>();
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
            await DatabaseService.client.GetTable<Users>().UpdateAsync(data);
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
