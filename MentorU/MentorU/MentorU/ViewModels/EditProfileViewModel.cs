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
    public class EditProfileViewModel : BaseViewModel
    {
        private User _user;
        public string Name { get; set; }
        public string Major { get; set; }
        public ObservableCollection<string> Classes { get; set; }
        public string Bio { get; set; }
        public Command SaveButtonCommand { get; set; }
        public Command CancelButtonCommand { get; set; }

        /***
         * Creates a temporary user that saves the changes. If cancelled the _user
         * remains the same. If save the temp values are copied over to the _user
         */
        public EditProfileViewModel()
        {
            // TODO: pull current user data from database or local cache
            _user = new User("Wallace");
            Name = _user.Name;
            Major = _user.Major;
            Bio = _user.Bio;
            SaveButtonCommand = new Command(OnSave);
            CancelButtonCommand = new Command(OnCancel);
        }

        public async void OnSave()
        {
            // TODO: update attributes to data base
            _user.Name = Name;
            _user.Major = Major;
            _user.Bio = Bio;

            await Shell.Current.GoToAsync("..");
        }

        public async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
