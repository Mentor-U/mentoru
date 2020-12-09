using MentorU.Models;
using MentorU.Views;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class EditProfileViewModel : ProfileViewModel
    {
        private Users _user;
        private ProfileViewModel _parentVM;
        public Command SaveButtonCommand { get; set; }
        public Command CancelButtonCommand { get; set; }

        /***
         * Allows for changes to the users profile and inherits the state of
         * the ProfileViewModel to allow the changes to be reflected in the ProfileView
         * if the are saved.
         */
        public EditProfileViewModel(ProfileViewModel profileVM)
        {
            _parentVM = profileVM;
            _user = DataStore.GetUser().Result;
            Name = _user.FirstName;
            Major = _user.Major;
            Bio = _user.Bio;
            SaveButtonCommand = new Command(OnSave);
            CancelButtonCommand = new Command(OnCancel);
        }

        private async void OnSave()
        {
            _user.FirstName = _parentVM.Name = Name;
            _user.Major = _parentVM.Major = Major;
            _user.Bio = _parentVM.Bio = Bio;
            await Shell.Current.Navigation.PopModalAsync();
        }

        private async void OnCancel()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }
    }
}
