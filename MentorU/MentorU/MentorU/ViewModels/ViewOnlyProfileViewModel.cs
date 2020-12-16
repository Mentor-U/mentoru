using MentorU.Models;
using MentorU.Views.ChatViews;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class ViewOnlyProfileViewModel : BaseViewModel
    {
        private string name;
        private string field;
        private string bio;
        private Users _user;
        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Field { get => field; set => SetProperty(ref field, value); }
        public string Bio { get => bio; set => SetProperty(ref bio, value); }

        public ViewOnlyProfileViewModel(Users user)
        {
            _user = user;
            Name = _user.FirstName;
            Field = _user.Major;
            Bio = _user.Bio;
        }

        public async void OnRequestMentor()
        {
            // TODO: handle requests to add a mentor
            await Application.Current.MainPage.DisplayAlert("You have sent a request to "+ _user.FirstName, "We'll let you know if they accept your request!", "OK");
        }

        public async void StartChat(object obj)
        {
            await Shell.Current.Navigation.PopToRootAsync(false); // false -> disables navigation animation
            await Shell.Current.Navigation.PushAsync(new ChatPage(_user));
        }

    }
}
