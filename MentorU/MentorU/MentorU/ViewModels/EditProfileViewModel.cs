﻿using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class EditProfileViewModel : ProfileViewModel
    {
        private ProfileViewModel _parentVM;
        private string _newClass;

        public Command SaveButtonCommand { get; set; }
        public Command CancelButtonCommand { get; set; }
        public Command AddClassCommand { get; set; }
        public Command RemoveClassCommand { get; set; }

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
            Name = App.ActiveUser.FirstName;
            Major = App.ActiveUser.Major;
            Bio = App.ActiveUser.Bio;
            Classes = _parentVM.Classes;

            AddClassCommand = new Command(AddClass);
            RemoveClassCommand = new Command(async () => await RemoveClass());
            SaveButtonCommand = new Command(OnSave);
            CancelButtonCommand = new Command(OnCancel);
        }

        private void AddClass()
        {
            Classes.Add(NewClass);
            NewClass = "";
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
            App.ActiveUser.FirstName = _parentVM.Name = Name;
            App.ActiveUser.Major = _parentVM.Major = Major;
            App.ActiveUser.Bio = _parentVM.Bio = Bio;
            _parentVM.Classes = Classes;
            await Shell.Current.Navigation.PopModalAsync();
        }

        private async void OnCancel()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }

    }
}
