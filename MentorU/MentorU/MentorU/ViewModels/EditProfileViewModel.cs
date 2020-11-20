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
        private User _tempChanges;
        public string Name { get => _user.Name; set => _tempChanges.Name = Name; }
        public string Major { get => _user.Major; set => _tempChanges.Major = Major; }
        public ObservableCollection<string> Classes { get; set; }
        public string Bio { get => _user.Bio; set => _tempChanges.Name = Bio; }
        public Command SaveButtonCommand { get; set; }
        public Command CancelButtonCommand { get; set; }

        /***
         * Creates a temporary user that saves the changes. If cancelled the _user
         * remains the same. If save the temp values are copied over to the _user
         */
        public EditProfileViewModel(User user)
        {
            _user = user;
            _tempChanges = new User("");
            SaveButtonCommand = new Command(OnSave);
            
        }

        public async void OnSave()
        {
            // update attributes
            _user.Name = _tempChanges.Name;
            _user.Major = _tempChanges.Major;
            _user.Classes = new List<string>(_tempChanges.Classes);
            _user.Bio = _tempChanges.Bio;

            // Navigate back to the profile page
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        public async void OnCancel()
        {
            // Disregard changes and go back
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
